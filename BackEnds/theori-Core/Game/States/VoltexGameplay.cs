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
using theori.Audio.CSCore;

namespace theori.Game.States
{
    class VoltexGameplay : State
    {
        private HighwayControl control;
        private HighwayView highwayView;
        private Panel foreUiRoot, backUiRoot;
        private CriticalLine critRoot;
        
        private Chart m_chart;
        private ChartPlayback m_playback;
        private CSCoreSource m_audio;

        private int actionKind = 0;

        public override void ClientSizeChanged(int width, int height)
        {
            highwayView.Camera.AspectRatio = Window.Aspect;
        }

        public override void Init()
        {
            Keyboard.KeyPress += KeyboardButtonPress;
            
            const string DIR = @"D:\kshootmania\songs\SDVX IV\two-torial";
           // const string DIR = @"D:\kshootmania\songs\Local\racemization";
            //const string DIR = @"D:\kshootmania\songs\Local\rocknroll";
            //const string DIR = @"D:\kshootmania\songs\Local\moonlightsonata";
            
            //var ksh = KShootMania.Chart.CreateFromFile(Path.Combine(DIR, "exh.ksh"));
            //var ksh = KShootMania.Chart.CreateFromFile(Path.Combine(DIR, "nov.ksh"));
            //var ksh = KShootMania.Chart.CreateFromFile(Path.Combine(DIR, "loc.ksh"));
            var ksh = KShootMania.Chart.CreateFromFile(Path.Combine(DIR, "mxm.ksh"));
            
            string audioFile = Path.Combine(DIR, ksh.Metadata.MusicFileNoFx ?? ksh.Metadata.MusicFile);
            m_audio = CSCoreSource.FromFile(audioFile);
            Application.Mixer.MasterChannel.AddSource(m_audio);

            m_audio.Play();

            m_chart = ksh.ToVoltex();

            highwayView = new HighwayView(m_chart);
            control = new HighwayControl();

            m_playback = new ChartPlayback(m_chart);
            m_playback.ObjectAppear += highwayView.RenderableObjectAppear;
            m_playback.ObjectDisappear += highwayView.RenderableObjectDisappear;
            m_playback.EventTrigger += PlaybackEventTrigger;

            //m_playback.ObjectBegin += (obj) => Trace.WriteLine("BEGIN: " + obj);
            //m_playback.ObjectEnd += (obj) => Trace.WriteLine("END: " + obj);

            highwayView.ViewDuration = m_playback.ViewDuration;

            foreUiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                    critRoot = new CriticalLine(),
                }
            };
        }

        private void PlaybackEventTrigger(Event evt)
        {
            switch (evt)
            {
                case LaserApplicationEvent app:
                {
                } break;
                
                case LaserParamsEvent pars:
                {
                } break;
                
                case SpinImpulseEvent spin: control.ApplySpin(spin.Params); break;
                case SwingImpulseEvent swing: control.ApplySwing(swing.Params); break;
                case WobbleImpulseEvent wobble: control.ApplyWobble(wobble.Params); break;
            }
        }

        private void KeyboardButtonPress(KeyInfo key)
        {
            switch (key.KeyCode)
            {
                case KeyCode.SPACE:
                {
                    if (m_audio.PlaybackState == PlaybackState.Stopped)
                        m_audio.Play();
                    else m_audio.Stop();
                } break;

                case KeyCode.D1: actionKind = 0; break;
                case KeyCode.D2: actionKind = 1; break;
                case KeyCode.D3: actionKind = 2; break;

                case KeyCode.LEFT: case KeyCode.RIGHT:
                {
                    int dir = key.KeyCode == KeyCode.LEFT ? -1 : 1;
                    switch (actionKind)
                    {
                        case 0: control.ApplySpin(new SpinParams()
                        {
                            Direction = (AngularDirection)dir,
                            Duration = 2.0,
                        }); break;
                    
                        case 1: control.ApplySwing(new SwingParams()
                        {
                            Direction = (AngularDirection)dir,
                            Duration = 1.0,
                            Amplitude = 45,
                        }); break;
                    
                        case 2: control.ApplyWobble(new WobbleParams()
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
        
        public override void Update()
        {
            time_t position = m_audio.Position;
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

            float GetTempRollValue(int stream, bool oneMinus = false)
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

            control.LeftLaserInput = GetTempRollValue(6);
            control.RightLaserInput = GetTempRollValue(7, true);
            
            control.Zoom = GetPathValueLerped((int)StreamIndex.Zoom);
            control.Pitch = GetPathValueLerped((int)StreamIndex.Pitch);
            control.Offset = GetPathValueLerped((int)StreamIndex.Offset) * 5 / 6.0f;
            control.Roll = GetPathValueLerped((int)StreamIndex.Roll);

            control.Update();
            control.ApplyToView(highwayView);
            
            highwayView.PlaybackPosition = position;
            highwayView.Update();

            critRoot.LaserRoll = highwayView.LaserRoll + control.Roll * 360;
            critRoot.EffectRoll = control.EffectRoll;
            critRoot.EffectOffset = control.EffectOffset;
            critRoot.HorizonHeight = highwayView.HorizonHeight;
            critRoot.CriticalHeight = highwayView.CriticalHeight;

            foreUiRoot.Update();
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
    }
}
