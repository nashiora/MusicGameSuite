using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using theori.Graphics;
using theori.Gui;
using theori.Input;
using OpenGL;
using OpenRM;
using OpenRM.Convert;
using OpenRM.Voltex;
using System.Diagnostics;
using theori.Audio;
using OpenRM.Audio.Effects;
using MoonSharp.Interpreter;
using theori.Configuration;

namespace theori.Game.States
{
    class VoltexGameplay : State
    {
        private LuaScript m_luaScript;

        private HighwayControl m_control;
        private HighwayView highwayView;
        private Panel foreUiRoot, backUiRoot;
        private CriticalLine critRoot;
        
        private Chart m_chart;
        #if OLD_PLAYBACK
        private SimpleChartPlayback m_playback;
        #else
        private SlidingChartPlayback m_playback;
        #endif

        private AudioEffectController m_audioController;
        private AudioTrack m_audio;
        private AudioSample m_slamSample;

        private int actionKind = 0;

        private bool m_isPlayback = false;
        private tick_t CurrentPositionTicks
        {
            get
            {
                time_t position = m_audioController.Position;
                var cp = m_chart.ControlPoints.MostRecent(position);

                position -= cp.AbsolutePosition;
                return (double)(position / cp.MeasureDuration);
            }
        }

        #region Edit Settings

        private static readonly int[] quantDivisions = new[] { 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192 };

        private int QuantizeIndex = 7;
        private int QuantizeDivision => quantDivisions[QuantizeIndex];

        #endregion

        public VoltexGameplay(Chart chart, AudioTrack audio)
        {
            m_chart = chart;
            m_audio = audio;

            Logger.Log(audio.Volume);
        }

        public override void ClientSizeChanged(int width, int height)
        {
            highwayView.Camera.AspectRatio = Window.Aspect;
        }

        public override void Init()
        {
            Keyboard.KeyPress += KeyboardButtonPress;

            m_luaScript = new LuaScript();
            m_luaScript["XShakeCamera"] = (Action<float>)(magnitude => m_control.ShakeCamera(magnitude));

            m_slamSample = AudioSample.FromFile(@"skins\Default\audio\slam.wav");
            m_slamSample.Channel = Host.Mixer.MasterChannel;
            m_slamSample.Volume = 0.5f * 0.7f;
            
            //m_audio.PlaybackSpeed = 1.25f;

            m_audioController = new AudioEffectController(8, m_audio, true)
            {
                RemoveFromChannelOnFinish = true,
            };
            m_audioController.Finish += () =>
            {
                Console.WriteLine("track complete");
            };
            
            m_audio.Position = m_chart.Offset;

            highwayView = new HighwayView(m_chart);
            m_control = new HighwayControl();
            
            m_playback = new SlidingChartPlayback(m_chart);
            m_playback.ObjectHeadCrossPrimary += (dir, obj) =>
            {
                if (dir == PlayDirection.Forward)
                    highwayView.RenderableObjectAppear(obj);
                else highwayView.RenderableObjectDisappear(obj);
            };
            m_playback.ObjectTailCrossSecondary += (dir, obj) =>
            {
                if (dir == PlayDirection.Forward)
                    highwayView.RenderableObjectDisappear(obj);
                else highwayView.RenderableObjectAppear(obj);
            };

            // TODO(local): Effects wont work with backwards motion, but eventually the
            //  editor (with the only backwards motion support) will pre-render audio instead.
            m_playback.ObjectHeadCrossCritical += (dir, obj) =>
            {
                if (dir != PlayDirection.Forward) return;

                if (obj is Event evt)
                    PlaybackEventTrigger(evt);
                else PlaybackObjectBegin(obj);
            };
            m_playback.ObjectTailCrossCritical += (dir, obj) => PlaybackObjectEnd(obj);

            m_playback.LookAhead *= m_audio.PlaybackSpeed;
            highwayView.ViewDuration = m_playback.LookAhead;

            foreUiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                    critRoot = new CriticalLine(),

                    new Panel() // buttons
                    {
                        Size = Vector2.One,
                        RelativeSizeAxes = Axes.Both,

                        Children = new GuiElement[]
                        {
                            new Sprite(null)
                            {
                                Size = new Vector2(32, 32),
                                Position = new Vector2(12, 12),
                            },
                        }
                    },
                }
            };
            
            m_luaScript.DoString(@"
event = {};

function event.gp_slam_play(slam_magnitude)
    XShakeCamera(-math.sign(slam_magnitude));
end
");
        }

        private void ResetPlayback()
        {
            time_t minStartTime = m_chart.TimeEnd;
            for (int i = 0; i < 8; i++)
            {
                time_t startTime = m_chart[i].FirstObject?.AbsolutePosition ?? 0;
                if (startTime < minStartTime)
                    minStartTime = startTime;
            }

            minStartTime -= 3;
            if (minStartTime < 0)
                m_audio.Position = minStartTime;
            else m_audio.Position = 0;
            
            m_audioController.Play();
        }

        private DynValue InvokeLuaEvent(string eventName, params object[] values)
        {
            var eventTable = m_luaScript["event"] as Table;
            return m_luaScript.Call(eventTable[eventName], values);
        }

        private void PlaybackObjectBegin(OpenRM.Object obj)
        {
            if (obj is AnalogObject aobj)
            {
                if (obj.IsInstant)
                {
                    InvokeLuaEvent("gp_slam_play", aobj.FinalValue - aobj.InitialValue);
                    if (aobj.InitialValue == (aobj.Stream == 6 ? 0 : 1) && aobj.NextConnected == null)
                        m_control.ApplyRollImpulse(MathL.Sign(aobj.FinalValue - aobj.InitialValue));
                    m_slamSample.Play();
                }

                if (aobj.PreviousConnected == null)
                {
                    if (!AreLasersActive) m_audioController.SetEffect(6, currentLaserEffectDef, BASE_LASER_MIX);
                    currentActiveLasers[obj.Stream - 6] = true;
                }
            }
            else if (obj is ButtonObject bobj)
            {
                if (bobj.HasEffect)
                    m_audioController.SetEffect(obj.Stream, bobj.Effect);
                else m_audioController.RemoveEffect(obj.Stream);
            }
        }

        private void PlaybackObjectEnd(OpenRM.Object obj)
        {
            if (obj is AnalogObject aobj)
            {
                if (aobj.NextConnected == null)
                {
                    currentActiveLasers[obj.Stream - 6] = false;
                    if (!AreLasersActive) m_audioController.RemoveEffect(6);
                }
            }
            if (obj is ButtonObject bobj)
            {
                m_audioController.RemoveEffect(obj.Stream);
            }
        }

        private void PlaybackEventTrigger(Event evt)
        {
            switch (evt)
            {
                case LaserApplicationEvent app: m_control.LaserApplication = app.Application; break;
                
                // TODO(local): left/right lasers separate + allow both independent if needed
                case LaserFilterGainEvent filterGain: laserGain = filterGain.Gain; break;
                case LaserFilterKindEvent filterKind:
                {
                    m_audioController.SetEffect(6, currentLaserEffectDef = filterKind.FilterEffect, m_audioController.GetEffectMix(6));
                } break;

                case LaserParamsEvent pars:
                {
                    if (pars.LaserIndex.HasFlag(LaserIndex.Left))  m_control.LeftLaserParams = pars.Params;
                    if (pars.LaserIndex.HasFlag(LaserIndex.Right)) m_control.RightLaserParams = pars.Params;
                } break;
                
                case SlamVolumeEvent pars: m_slamSample.Volume = pars.Volume * 0.7f; break;

                case SpinImpulseEvent spin: m_control.ApplySpin(spin.Params); break;
                case SwingImpulseEvent swing: m_control.ApplySwing(swing.Params); break;
                case WobbleImpulseEvent wobble: m_control.ApplyWobble(wobble.Params); break;
            }
        }

        private void KeyboardButtonPress(KeyInfo key)
        {
            var cp = m_chart.ControlPoints.MostRecent(m_audioController.Position);

            switch (key.KeyCode)
            {
                case KeyCode.SPACE:
                {
                    if (m_audioController.PlaybackState == PlaybackState.Stopped)
                        m_audioController.Play();
                    else
                    {
                        // TODO(local): Effects are gonna have to work from any point ono
                        m_audioController.Stop();
                        m_audioController.Position = GetQuantizedTime(m_audioController.Position); 
                    }
                } break;

                case KeyCode.RETURN:
                {
                    time_t minStartTime = m_chart.TimeEnd;
                    for (int i = 0; i < 8; i++)
                    {
                        time_t startTime = m_chart[i].FirstObject?.AbsolutePosition ?? 0;
                        if (startTime < minStartTime)
                            minStartTime = startTime;
                    }

                    minStartTime -= 2;
                    if (minStartTime > m_audioController.Position)
                        m_audioController.Position = minStartTime;
                } break;

                case KeyCode.PAGEUP: m_audioController.Position += cp.MeasureDuration; break;
                case KeyCode.PAGEDOWN: m_audioController.Position -= cp.MeasureDuration; break;

                case KeyCode.UP: m_audioController.Position += cp.QuarterNoteDuration * 4 / QuantizeDivision; break;
                case KeyCode.DOWN: m_audioController.Position -= cp.QuarterNoteDuration * 4 / QuantizeDivision; break;
                    
                case KeyCode.D1: InsertButton(0, CurrentPositionTicks); break;
                case KeyCode.D2: InsertButton(1, CurrentPositionTicks); break;
                case KeyCode.D3: InsertButton(2, CurrentPositionTicks); break;
                case KeyCode.D4: InsertButton(3, CurrentPositionTicks); break;

                case KeyCode.LEFT: case KeyCode.RIGHT:
                {
                    int dir = key.KeyCode == KeyCode.LEFT ? -1 : 1;
                    switch (actionKind)
                    {
                        case 0: m_control.ApplySpin(new SpinParams()
                        {
                            Direction = (AngularDirection)dir,
                            Duration = 2.0,
                        }); break;
                    
                        case 1: m_control.ApplySwing(new SwingParams()
                        {
                            Direction = (AngularDirection)dir,
                            Duration = 1.0,
                            Amplitude = 45,
                        }); break;
                    
                        case 2: m_control.ApplyWobble(new WobbleParams()
                        {
                            Direction = (LinearDirection)dir,
                            Duration = 1.0,
                            Amplitude = 1,
                            Decay = Decay.On,
                            Frequency = 3,
                        }); break;
                    }
                } break;
                
                case KeyCode.F5:
                {
                    ResetPlayback();
                } break;
            }
        }

        private void InsertButton(int sIdx, tick_t pos)
        {
            var stream = m_chart[sIdx];
            var mostRecent = stream.MostRecent<ButtonObject>(pos);

            if (mostRecent != null)
            {
                if (mostRecent.Position == pos || mostRecent.IsChip)
                {
                    m_playback.RemoveObject(mostRecent);
                    stream.Remove(mostRecent);
                    return; // don't place a new one here
                }
                else if (mostRecent.EndPosition >= pos)
                    // TODO(local): disallowing only makes sense for chips, but holds should still be allowed
                    return; // don't allow the placement, it's at the end of a hold
            }

            var bObj = stream.Add<ButtonObject>(pos);
            m_playback.AddObject(bObj);
        }

        private void RemoveButton(ButtonObject bObj)
        {
            m_playback.RemoveObject(bObj);
            m_chart[bObj.Stream].Remove(bObj);
        }

        private float GetTempRollValue(time_t position, int stream, bool oneMinus = false)
        {
            var s = m_playback.Chart[stream];

            var mrAnalog = s.MostRecent<AnalogObject>(position);
            if (mrAnalog == null || position > mrAnalog.AbsoluteEndPosition)
                return 0;

            float result = mrAnalog.SampleValue(position);
            if (oneMinus)
                return 1 - result;
            else return result;
        }
        
        private time_t GetQuantizedTime(time_t position)
        {
            var cp = m_chart.ControlPoints.MostRecent(position);
            time_t remaining = position - cp.AbsolutePosition;
            time_t quantizeDuration = cp.BeatDuration * 4 / QuantizeDivision;

            int numSteps = (int)(remaining / quantizeDuration);

            return cp.AbsolutePosition + quantizeDuration * numSteps;
        }

        public override void Update()
        {
            time_t position = m_audio.Position;
            m_luaScript["audioTime"] = position.Seconds;

            m_control.Position = position;
            m_playback.Position = position;

            float GetPathValueLerped(int stream)
            {
                var s = m_playback.Chart[stream];

                var mrPoint = s.MostRecent<PathPointEvent>(position);
                if (mrPoint == null)
                    return ((PathPointEvent)s.FirstObject)?.Value ?? 0;

                if (mrPoint.HasNext)
                {
                    float alpha = (float)((position - mrPoint.AbsolutePosition).Seconds / (mrPoint.Next.AbsolutePosition - mrPoint.AbsolutePosition).Seconds);
                    return MathL.Lerp(mrPoint.Value, ((PathPointEvent)mrPoint.Next).Value, alpha);
                }
                else return mrPoint.Value;
            }

            m_control.MeasureDuration = m_chart.ControlPoints.MostRecent(position).MeasureDuration;

            m_control.LeftLaserInput = GetTempRollValue(position, 6);
            m_control.RightLaserInput = GetTempRollValue(position, 7, true);
            
            m_control.Zoom = GetPathValueLerped(StreamIndex.Zoom);
            m_control.Pitch = GetPathValueLerped(StreamIndex.Pitch);
            m_control.Offset = GetPathValueLerped(StreamIndex.Offset) * 5 / 6.0f;
            m_control.Roll = GetPathValueLerped(StreamIndex.Roll);

            m_control.Update();
            m_control.ApplyToView(highwayView);
            
            highwayView.PlaybackPosition = position;
            highwayView.Update();
            
            critRoot.LaserRoll = highwayView.LaserRoll;
            critRoot.BaseRoll = m_control.Roll * 360;
            critRoot.EffectRoll = m_control.EffectRoll;
            critRoot.EffectOffset = m_control.EffectOffset;
            // TODO(local): Adding this stuff doesn't FIX the problem but it's almost entirely NOT noticeable so uh fix it for real yeah
            // NOTE(local): The problem in question is that the HorizonHeight doesn't match properly for the CritLine and oh well
            critRoot.HorizonHeight = highwayView.HorizonHeight + (Window.Height - highwayView.CriticalHeight) / 2;
            critRoot.CriticalHeight = highwayView.CriticalHeight;

            foreUiRoot.Update();

            UpdateEffects();
            m_audioController.EffectsActive = true;
        }
        
        public override void Render()
        {
            void DrawUiRoot(Panel root)
            {
                if (root == null) return;

                var viewportSize = new Vector2(Window.Width, Window.Height);
                using (var grq = new GuiRenderQueue(viewportSize))
                {
                    root.Position = Vector2.Zero;
                    root.RelativeSizeAxes = Axes.None;
                    root.Size = viewportSize;
                    root.Rotation = 0;
                    root.Scale = Vector2.One;
                    root.Origin = Vector2.Zero;

                    root.Render(grq);
                }
            }

            DrawUiRoot(backUiRoot);
            highwayView.Render();
            DrawUiRoot(foreUiRoot);
        }

        private void UpdateEffects()
        {
            UpdateLaserEffects();
        }

        private EffectDef currentLaserEffectDef = EffectDef.GetDefault(EffectType.PeakingFilter);
        private readonly bool[] currentActiveLasers = new bool[2];
        private readonly float[] currentActiveLaserAlphas = new float[2];

        private bool AreLasersActive => currentActiveLasers[0] || currentActiveLasers[1];

        private const float BASE_LASER_MIX = 0.7f;
        private float laserGain = 0.5f;

        private void UpdateLaserEffects()
        {
            if (!AreLasersActive)
            {
                m_audioController.SetEffectMix(6, 0);
                return;
            }

            float LaserAlpha(int index)
            {
                return GetTempRollValue(m_audio.Position, index + 6, index == 1);
            }
            
            if (currentActiveLasers[0])
                currentActiveLaserAlphas[0] = LaserAlpha(0);
            if (currentActiveLasers[1])
                currentActiveLaserAlphas[1] = LaserAlpha(1);
            
            float alpha;
            if (currentActiveLasers[0] && currentActiveLasers[1])
                alpha = Math.Max(currentActiveLaserAlphas[0], currentActiveLaserAlphas[1]);
            else if (currentActiveLasers[0])
                alpha = currentActiveLaserAlphas[0];
            else alpha = currentActiveLaserAlphas[1];

            m_audioController.UpdateEffect(6, alpha);

            float mix = BASE_LASER_MIX * laserGain;
            if (currentLaserEffectDef != null && currentLaserEffectDef.Type == EffectType.PeakingFilter)
            {
                if (alpha < 0.1f)
                    mix *= alpha / 0.1f;
                else if (alpha > 0.8f)
                    mix *= 1 - (alpha - 0.8f) / 0.2f;
            }

            m_audioController.SetEffectMix(6, mix);
        }
    }
}
