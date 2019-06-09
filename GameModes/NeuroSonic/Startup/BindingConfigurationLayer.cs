using System;
using System.Numerics;
using theori;
using theori.Configuration;
using theori.Graphics;
using theori.Gui;
using theori.IO;

namespace NeuroSonic.Startup
{
    public sealed class ControllerConfigurationLayer : BaseMenuLayer
    {
        class Bindable : Panel
        {
            private Sprite m_fill;
            private TextLabel m_label;

            public Vector4 FillColor { set => m_fill.Color = value; }
            public string Text { set => m_label.Text = value; }

            public Bindable(string name)
            {
                Children = new GuiElement[]
                {
                    m_fill = new Sprite(null)
                    {
                        RelativeSizeAxes = Axes.Both,

                        Color = Vector4.One,
                        Size = Vector2.One,
                    },

                    new TextLabel(Font.Default24, name)
                    {
                        RelativePositionAxes = Axes.Both,

                        Color = new Vector4(0, 0, 0, 1),
                        Position = new Vector2(0.5f, 0.25f),
                        TextAlignment = TextAlignment.MiddleCenter,
                    },

                    m_label = new TextLabel(Font.Default24, "<?>")
                    {
                        RelativePositionAxes = Axes.Both,

                        Color = new Vector4(0, 0, 0, 1),
                        Position = new Vector2(0.5f, 0.75f),
                        TextAlignment = TextAlignment.MiddleCenter,
                    },
                };
            }
        }

        protected override string Title => "Controller Binding Configuration";

        private int m_whichEdit = -1;
        private Panel m_graphicPanel;

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(ItemIndex, "Configure Primary Bindings", () => m_whichEdit = 0));
            AddMenuItem(new MenuItem(ItemIndex, "Configure Secondary Bindings", () => m_whichEdit = 1));
            AddMenuItem(new MenuItem(ItemIndex, "Other Misc. Bindings", () => { }));
        }

        public override void Init()
        {
            base.Init();

            GuiRoot.AddChild(m_graphicPanel = new Panel()
            {
                Children = new GuiElement[]
                {
                    new Panel()
                    {
                        RelativeSizeAxes = Axes.Both,
                        RelativePositionAxes = Axes.Both,

                        Size = new Vector2(1.0f, 1 / 0.75f),
                        Position = new Vector2(0.0f, -0.25f),

                        Children = new GuiElement[]
                        {
                            new Bindable("Start")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.45f, 0.2f),
                                Size = new Vector2(0.1f, 0.1f),
                            },

                            new Bindable("Back")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.45f, 0.7f),
                                Size = new Vector2(0.1f, 0.1f),
                            },

                            new Bindable("BT-A")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.025f, 0.4f),
                                Size = new Vector2(0.2f, 0.2f),
                            },

                            new Bindable("BT-B")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.275f, 0.4f),
                                Size = new Vector2(0.2f, 0.2f),
                            },

                            new Bindable("BT-C")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.525f, 0.4f),
                                Size = new Vector2(0.2f, 0.2f),
                            },

                            new Bindable("BT-D")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.775f, 0.4f),
                                Size = new Vector2(0.2f, 0.2f),
                            },

                            new Bindable("FX-L")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.175f, 0.65f),
                                Size = new Vector2(0.175f, 0.1f),
                            },

                            new Bindable("FX-R")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.675f, 0.65f),
                                Size = new Vector2(0.175f, 0.1f),
                            },

                            new Bindable("VOL-L")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.2f, 0.25f),
                                Size = new Vector2(0.1f, 0.1f),
                            },

                            new Bindable("VOL-R")
                            {
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,

                                Position = new Vector2(0.7f, 0.25f),
                                Size = new Vector2(0.1f, 0.1f),
                            },
                        }
                    },
                }
            });
        }

        public override bool KeyPressed(KeyInfo key)
        {
            // when NOT editing a config, do the base menu layer inputs
            if (m_whichEdit == -1)
                return base.KeyPressed(key);

            switch (key.KeyCode)
            {
                case KeyCode.ESCAPE: m_whichEdit = -1; break;

                default: return false;
            }

            return true;
        }

        public override void Update(float delta, float total)
        {
            int size = (int)MathL.Max(Window.Width * 0.6f, Window.Height * 0.6f);
            m_graphicPanel.Size = new Vector2(size, size * 0.75f);
            m_graphicPanel.Position = new Vector2((Window.Width - size) / 2, Window.Height - size * 0.75f);
        }
    }
}
