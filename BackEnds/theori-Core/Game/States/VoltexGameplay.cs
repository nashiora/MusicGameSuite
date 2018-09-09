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
        private ChartPlayback m_playback;

        private AudioEffectController m_audioController;
        private AudioTrack m_audio;
        private AudioSample m_slamSample;

        private int actionKind = 0;

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
            m_slamSample.Volume = 0.5f * 0.7f;
            Application.Mixer.MasterChannel.AddSource(m_slamSample);
            
            const string DIR = @"D:\kshootmania\songs\SDVX IV\two-torial";
            //const string DIR = @"D:\kshootmania\songs\Local\racemization";
            //const string DIR = @"D:\kshootmania\songs\Local\rocknroll";
            //const string DIR = @"D:\kshootmania\songs\Local\moonlightsonata";
            
            var ksh = KShootMania.Chart.CreateFromFile(Path.Combine(DIR, "exh.ksh"));
            //var ksh = KShootMania.Chart.CreateFromFile(Path.Combine(DIR, "nov.ksh"));
            //var ksh = KShootMania.Chart.CreateFromFile(Path.Combine(DIR, "loc.ksh"));
            //var ksh = KShootMania.Chart.CreateFromFile(Path.Combine(DIR, "mxm.ksh"));
            
            string audioFile = Path.Combine(DIR, ksh.Metadata.MusicFileNoFx ?? ksh.Metadata.MusicFile);

            m_audio = AudioTrack.FromFile(audioFile);
            m_audio.Channel = Application.Mixer.MasterChannel;

            m_audioController = new AudioEffectController(8, m_audio, true)
            {
                RemoveFromChannelOnFinish = true,
            };
            m_audioController.Finish += () =>
            {
                Console.WriteLine("track complete");
            };
            
            m_chart = ksh.ToVoltex();

            time_t minStartTime = m_chart.TimeEnd;
            for (int i = 0; i < 8; i++)
            {
                time_t startTime = m_chart[i].FirstObject?.AbsolutePosition ?? 0;
                if (startTime < minStartTime)
                    minStartTime = startTime;
            }

            minStartTime -= 3;
            if (minStartTime < 0)
            {
                m_audio.Position = minStartTime;   
                Console.WriteLine($"{ minStartTime }, { m_audio.Position }");
            }

            highwayView = new HighwayView(m_chart);
            m_control = new HighwayControl();

            m_playback = new ChartPlayback(m_chart);
            m_playback.ObjectAppear += highwayView.RenderableObjectAppear;
            m_playback.ObjectDisappear += highwayView.RenderableObjectDisappear;
            m_playback.EventTrigger += PlaybackEventTrigger;

            m_playback.ObjectBegin += PlaybackObjectBegin;
            m_playback.ObjectEnd += PlaybackObjectEnd;

            highwayView.ViewDuration = m_playback.ViewDuration;

            foreUiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                    critRoot = new CriticalLine(),
                }
            };
            
            m_luaScript.DoString(@"
function OnSlamHit(magnitude)
    XShakeCamera(-math.sign(magnitude));
end
");
            
            m_audioController.Play();
        }

        private void PlaybackObjectBegin(OpenRM.Object obj)
        {
            if (obj is AnalogObject aobj)
            {
                if (obj.IsInstant)
                {
                    m_luaScript.Call("OnSlamHit", aobj.FinalValue - aobj.InitialValue);
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
                case LaserFilterKindEvent filterKind: m_audioController.SetEffect(6, currentLaserEffectDef = filterKind.FilterEffect, m_audioController.GetEffectMix(6)); break;

                case LaserParamsEvent pars:
                {
                    if (pars.LaserIndex.HasFlag(LaserIndex.Left))  m_control.LeftLaserParams = pars.Params;
                    if (pars.LaserIndex.HasFlag(LaserIndex.Right)) m_control.RightLaserParams = pars.Params;
                } break;
                
                case SlamVolumeEvent pars: m_slamSample.Volume = pars.Volume * 0.7f; break;

                case SpinImpulseEvent spin: m_control.ApplySpin(spin.Params); break;
                case SwingImpulseEvent swing: m_control.ApplySwing(swing.Params); break;
                case WobbleImpulseEvent wobble: m_control.ApplyWobble(wobble.Params); break;

                //default: Console.WriteLine($"Skipping Event: { evt }"); break;
            }
        }

        private void KeyboardButtonPress(KeyInfo key)
        {
            switch (key.KeyCode)
            {
                case KeyCode.SPACE:
                {
                    if (m_audioController.PlaybackState == PlaybackState.Stopped)
                        m_audioController.Play();
                    else m_audioController.Stop();
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

                case KeyCode.PAGEUP: m_audioController.Position += 5; break;

                case KeyCode.D1: actionKind = 0; break;
                case KeyCode.D2: actionKind = 1; break;
                case KeyCode.D3: actionKind = 2; break;

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
            }
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
        
        public override void Update()
        {
            time_t position = m_audio.Position;
            //Console.WriteLine($"{ Time.Total } :: { position }");

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

            m_control.LeftLaserInput = GetTempRollValue(position, 6);
            m_control.RightLaserInput = GetTempRollValue(position, 7, true);
            
            m_control.Zoom = GetPathValueLerped((int)StreamIndex.Zoom);
            m_control.Pitch = GetPathValueLerped((int)StreamIndex.Pitch);
            m_control.Offset = GetPathValueLerped((int)StreamIndex.Offset) * 5 / 6.0f;
            m_control.Roll = GetPathValueLerped((int)StreamIndex.Roll);

            m_control.Update();
            m_control.ApplyToView(highwayView);
            
            highwayView.PlaybackPosition = position;
            highwayView.Update();

            critRoot.LaserRoll = highwayView.LaserRoll + m_control.Roll * 360;
            critRoot.EffectRoll = m_control.EffectRoll;
            critRoot.EffectOffset = m_control.EffectOffset;
            critRoot.HorizonHeight = highwayView.HorizonHeight;
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
                return;

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
            if (alpha < 0.1f)
                mix *= alpha / 0.1f;

            else if (alpha > 0.8f)
                mix *= 1 - (alpha - 0.8f) / 0.2f;

            m_audioController.SetEffectMix(6, mix);
        }
    }
}
