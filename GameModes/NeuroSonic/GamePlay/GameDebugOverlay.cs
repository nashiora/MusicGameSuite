using System;
using System.Collections.Generic;
using System.Numerics;
using NeuroSonic.IO;
using OpenGL;
using theori;
using theori.Graphics;
using theori.Gui;

namespace NeuroSonic.GamePlay
{
    public sealed class GameDebugOverlay : Overlay
    {
        private Panel m_root;

        private ControllerVisualizer m_visualizer;

        internal GameDebugOverlay(Controller controller)
        {
            m_root = new Panel()
            {
                Children = new GuiElement[]
                {
                    m_visualizer = new ControllerVisualizer(controller),
                }
            };
        }

        public override void Init()
        {
        }

        public override void Destroy()
        {
        }

        public override void Update(float delta, float total)
        {
            base.Update(delta, total);
            m_root.Update();
        }

        public override void Render()
        {
            if (m_root == null) return;

            var viewportSize = new Vector2(Window.Width, Window.Height);
            using (var grq = new GuiRenderQueue(viewportSize))
            {
                m_root.Position = Vector2.Zero;
                m_root.RelativeSizeAxes = Axes.None;
                m_root.Size = viewportSize;
                m_root.Rotation = 0;
                m_root.Scale = Vector2.One;
                m_root.Origin = Vector2.Zero;

                m_root.Render(grq);
            }
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

        public Controller Controller { get; }

        private readonly Dictionary<ControllerInput, ButtonSprite> m_bts = new Dictionary<ControllerInput, ButtonSprite>();
        private readonly Dictionary<ControllerInput, KnobSprite> m_knobs = new Dictionary<ControllerInput, KnobSprite>();

        public ControllerVisualizer(Controller controller)
        {
            Controller = controller;

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
                pair.Value.Active = Controller.IsButtonDown(pair.Key);

            foreach (var pair in m_knobs)
                pair.Value.Rotation = 360 * (1 + Controller.RawAxisValue(pair.Key)) * 0.5f;
        }
    }
}
