using System;
using System.Numerics;

using theori;
using theori.Graphics;
using theori.Gui;

using NeuroSonic.GamePlay;

namespace NeuroSonic.Startup
{
    class NeuroSonicStandaloneStartup : Layer
    {
        class MenuItem : TextLabel
        {
            public Action Action;

            public bool Hilited
            {
                set
                {
                    if (value)
                        Color = new Vector4(1, 1, 0, 1);
                    else Color = Vector4.One;
                }
            }

            public MenuItem(int i, string text, Action action)
                : base(Font.Default16, text)
            {
                Action = action;
                Position = new Vector2(0, 25 * i);
            }
        }

        private Panel m_guiRoot;

        private int m_itemIndex;
        private MenuItem[] m_items;

        public NeuroSonicStandaloneStartup()
        {
        }

        public override void Init()
        {
            Keyboard.KeyPress += KeyboardButtonPress;

            int i = 0;
            m_guiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                    new TextLabel(Font.Default24, "NeuroSonic Standalone Application")
                    {
                        Position = new Vector2(20, 20),
                    },

                    new Panel()
                    {
                        Position = new Vector2(40, 70),

                        Children = m_items = new MenuItem[]
                        {
                            new MenuItem(i++, "Event Mode", EnterEventMode),
                            new MenuItem(i++, "Demo Mode", EnterDemoMode),
                        }
                    },
                }
            };

            m_items[m_itemIndex].Hilited = true;
        }

        public override void Destroy()
        {
        }

        public override void Suspended()
        {
        }

        public override void Resumed()
        {
        }

        private void KeyboardButtonPress(KeyInfo key)
        {
            switch (key.KeyCode)
            {
                case KeyCode.UP:
                {
                    m_items[m_itemIndex].Hilited = false;
                    m_itemIndex = Math.Max(0, m_itemIndex - 1);
                    m_items[m_itemIndex].Hilited = true;
                } break;

                case KeyCode.DOWN:
                {
                    m_items[m_itemIndex].Hilited = false;
                    m_itemIndex = Math.Min(m_items.Length - 1, m_itemIndex + 1);
                    m_items[m_itemIndex].Hilited = true;
                }
                break;

                case KeyCode.RETURN:
                {
                    m_items[m_itemIndex].Action();
                } break;
            }
        }

        private void EnterEventMode()
        {
            Host.PushLayer(new GameLayer(true));
        }

        private void EnterDemoMode()
        {
        }

        public override void Update(float delta, float total)
        {
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

            DrawUiRoot(m_guiRoot);
        }
    }
}
