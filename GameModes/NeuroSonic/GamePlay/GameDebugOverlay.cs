using System;
using System.Collections.Generic;
using System.Numerics;
using NeuroSonic.GamePlay.Scoring;
using NeuroSonic.IO;
using OpenGL;
using OpenRM;
using theori;
using theori.Graphics;
using theori.Gui;

namespace NeuroSonic.GamePlay
{
    public sealed class GameDebugOverlay : Overlay
    {
        private ControllerVisualizer m_visualizer;
        private TimingBar m_timingBar;

        internal GameDebugOverlay()
        {
            ForegroundGui = new Panel()
            {
                Children = new GuiElement[]
                {
                    m_visualizer = new ControllerVisualizer(),
                    m_timingBar = new TimingBar()
                    {
                        RelativePositionAxes = Axes.Both,
                        RelativeSizeAxes = Axes.Both,

                        Position = new Vector2(0.3f, 0.95f),
                        Size = new Vector2(0.4f, 0.05f),
                    },
                }
            };
        }

        public void AddTimingInfo(time_t timingDelta, JudgeKind kind)
        {
            m_timingBar.AddTimingInfo(timingDelta, kind);
        }

        public override void Init()
        {
        }

        public override void Destroy()
        {
        }

        public override void Update(float delta, float total)
        {
        }

        public override void Render()
        {
        }
    }

    public class ControllerVisualizer : Panel
    {
        class ButtonSprite : Panel
        {
            private Sprite m_inactive, m_active;

            public bool Active
            {
                set
                {
                    m_inactive.Color = value ? Vector4.Zero : Vector4.One;
                    m_active.Color = value ? Vector4.One : Vector4.Zero;
                }
            }

            public ButtonSprite(string buttonName)
            {
                var itex = Texture.FromStream2D(typeof(ButtonSprite).Assembly.GetManifestResourceStream($"NeuroSonic.textures.debug_{buttonName}.png"));
                var atex = Texture.FromStream2D(typeof(ButtonSprite).Assembly.GetManifestResourceStream($"NeuroSonic.textures.debug_{buttonName}_active.png"));

                Children = new GuiElement[]
                {
                    m_inactive = new Sprite(itex)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = Vector2.One,
                        Color = Vector4.One,
                    },

                    m_active = new Sprite(atex)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = Vector2.One,
                        Color = new Vector4(1, 1, 0, 1),
                    },
                };

                Active = false;
            }
        }

        class KnobSprite : Panel
        {
            private Sprite m_sprite;

            public KnobSprite()
            {
                var tex = Texture.FromStream2D(typeof(ButtonSprite).Assembly.GetManifestResourceStream($"NeuroSonic.textures.debug_vol.png"));
                Children = new GuiElement[]
                {
                    m_sprite = new Sprite(tex)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = Vector2.One,
                        Color = Vector4.One,
                    }
                };
            }

            public override void Update()
            {
                base.Update();
                Origin = Size / 2;
            }
        }

        private readonly Dictionary<ControllerInput, ButtonSprite> m_bts = new Dictionary<ControllerInput, ButtonSprite>();
        private readonly Dictionary<ControllerInput, KnobSprite> m_knobs = new Dictionary<ControllerInput, KnobSprite>();

        public ControllerVisualizer()
        {
            Size = new Vector2(230, 120);
            Children = new GuiElement[]
            {
                new Sprite(null)
                {
                    Color = Vector4.One * 0.5f,
                    RelativeSizeAxes = Axes.Both,
                    Size = Vector2.One,
                },

                m_bts[ControllerInput.Start] = new ButtonSprite("bt")
                {
                    Position = new Vector2(107, 10),
                    Size = new Vector2(16),
                },

                m_bts[ControllerInput.Back] = new ButtonSprite("bt")
                {
                    Position = new Vector2(107, 94),
                    Size = new Vector2(16),
                },

                m_bts[ControllerInput.BT0] = new ButtonSprite("bt")
                {
                    Position = new Vector2(40, 50),
                    Size = new Vector2(30),
                },

                m_bts[ControllerInput.BT1] = new ButtonSprite("bt")
                {
                    Position = new Vector2(80, 50),
                    Size = new Vector2(30),
                },

                m_bts[ControllerInput.BT2] = new ButtonSprite("bt")
                {
                    Position = new Vector2(120, 50),
                    Size = new Vector2(30),
                },

                m_bts[ControllerInput.BT3] = new ButtonSprite("bt")
                {
                    Position = new Vector2(160, 50),
                    Size = new Vector2(30),
                },

                m_bts[ControllerInput.FX0] = new ButtonSprite("fx")
                {
                    Position = new Vector2(55, 90),
                    Size = new Vector2(30, 15),
                },

                m_bts[ControllerInput.FX1] = new ButtonSprite("fx")
                {
                    Position = new Vector2(140, 90),
                    Size = new Vector2(30, 15),
                },

                m_knobs[ControllerInput.Laser0Axis] = new KnobSprite()
                {
                    Position = new Vector2(20, 30),
                    Size = new Vector2(25),
                },

                m_knobs[ControllerInput.Laser1Axis] = new KnobSprite()
                {
                    Position = new Vector2(210, 30),
                    Size = new Vector2(25),
                },
            };
        }

        public override void Update()
        {
            base.Update();

            foreach (var pair in m_bts)
                pair.Value.Active = Input.IsButtonDown(pair.Key);

            foreach (var pair in m_knobs)
                pair.Value.Rotation = 360 * (1 + Input.RawAxisValue(pair.Key)) * 0.5f;
        }
    }
    
    public class TimingBar : Panel
    {
        private readonly time_t m_inaccuracyWindow = 150.0 / 1000;

        private int m_numInputs;
        private time_t m_totalInaccuracy;

        private Sprite m_cursor;

        public TimingBar()
        {
            Children = new GuiElement[]
            {
                new Sprite(null)
                {
                    RelativePositionAxes = Axes.Both,
                    RelativeSizeAxes = Axes.Both,
                    Position = new Vector2(0, 0.25f),
                    Size = new Vector2(1.0f, 0.5f),
                    Color = Vector4.One,
                },

                new Sprite(null)
                {
                    RelativePositionAxes = Axes.Both,
                    RelativeSizeAxes = Axes.Y,
                    Origin = new Vector2(2, 0),
                    Position = new Vector2(0.5f, 0),
                    Size = new Vector2(4, 1.0f),
                    Color = new Vector4(0, 0, 0, 1),
                },

                m_cursor = new Sprite(null)
                {
                    RelativePositionAxes = Axes.Both,
                    RelativeSizeAxes = Axes.Y,
                    Origin = new Vector2(5, 0),
                    Size = new Vector2(10, 1.0f),
                    Color = new Vector4(0, 1, 0, 1),
                }
            };
        }

        public void AddTimingInfo(time_t timingDelta, JudgeKind kind)
        {
            m_numInputs++;
            m_totalInaccuracy += timingDelta;
        }

        public override void Update()
        {
            base.Update();

            if (m_numInputs == 0)
                m_cursor.Position = new Vector2(0.5f, 0);
            else
            {
                time_t inacc = m_totalInaccuracy / m_numInputs;
                float alpha = (float)(inacc / m_inaccuracyWindow);

                m_cursor.Position = new Vector2((1 + alpha) * 0.5f, 0);
            }
        }
    }
}
