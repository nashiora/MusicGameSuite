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
                : base(Font.Default24, text)
            {
                Action = action;
                Position = new Vector2(0, 35 * i);
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
                    new TextLabel(Font.Default32, "NeuroSonic Standalone Application")
                    {
                        Position = new Vector2(20, 20),
                    },

                    new Panel()
                    {
                        Position = new Vector2(40, 100),

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
            var gameLayer = new GameLayer(AutoPlay.None);
            Host.PushLayer(gameLayer, _ => gameLayer.OpenChart());
        }

        private void EnterDemoMode()
        {
            var gameLayer = new GameLayer(AutoPlay.ButtonsAndLasers);
            // TODO(local): add demo controller as new layer on top!
            // Demo controller could also subclas or something, but I want to make sure
            //  this kind of use-case works. An otherwise invisible layer controls the parent
            //  layer silently, and is still handled properly as if it wasn't there c:
            // NOT an overlay, though, overlays are special and will ofc work if considered.
            Host.PushLayer(gameLayer);
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
