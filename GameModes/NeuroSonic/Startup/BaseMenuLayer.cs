using System;
using System.Collections.Generic;
using System.Numerics;

using theori;
using theori.Graphics;
using theori.Gui;
using theori.IO;

namespace NeuroSonic.Startup
{
    public class MenuItem : TextLabel
    {
        public const int SPACING = 35;

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
            Position = new Vector2(0, SPACING * i);
        }
    }

    public abstract class BaseMenuLayer : Layer
    {
        protected Panel GuiRoot;

        protected int ItemIndex { get; private set; }
        private readonly List<MenuItem> m_items = new List<MenuItem>();

        private int m_extraSpacing = 0;

        protected int NextOffset => m_items.Count + m_extraSpacing;

        protected abstract string Title { get; }

        public override void Init()
        {
            GenerateMenuItems();

            GuiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                    new TextLabel(Font.Default32, Title)
                    {
                        RelativePositionAxes = Axes.X,
                        Position = new Vector2(0.5f, 20),
                        TextAlignment = TextAlignment.TopCenter,
                    },

                    new Panel()
                    {
                        Position = new Vector2(40, 100),

                        Children = m_items
                    },

                    new Panel()
                    {
                        RelativeSizeAxes = Axes.X,
                        RelativePositionAxes = Axes.Both,

                        Position = new Vector2(0, 1),
                        Size = new Vector2(1, 0),

                        Children = new GuiElement[]
                        {
                            new TextLabel(Font.Default16, "[Up / Down] or [BT-A / BT-B] Navigate Menu")
                            {
                                TextAlignment = TextAlignment.BottomLeft,
                                Position = new Vector2(10, -10),
                            },

                            new Panel()
                            {
                                RelativePositionAxes = Axes.X,

                                Position = new Vector2(1, 0),

                                Children = new GuiElement[]
                                {
                                    new TextLabel(Font.Default16, "Select Option [Enter] or [START]")
                                    {
                                        TextAlignment = TextAlignment.BottomRight,
                                        Position = new Vector2(-10, -10),
                                    },
                                }
                            },
                        }
                    }
                }
            };

            m_items[ItemIndex].Hilited = true;
        }

        protected abstract void GenerateMenuItems();

        protected void AddMenuItem(MenuItem item) => m_items.Add(item);
        protected void AddSpacing() => m_extraSpacing++;

        public override void Destroy()
        {
        }

        public override void Suspended()
        {
        }

        public override void Resumed()
        {
        }

        protected void NavigateUp()
        {
            m_items[ItemIndex].Hilited = false;
            ItemIndex = Math.Max(0, ItemIndex - 1);
            m_items[ItemIndex].Hilited = true;
        }

        protected void NavigateDown()
        {
            m_items[ItemIndex].Hilited = false;
            ItemIndex = Math.Min(m_items.Count - 1, ItemIndex + 1);
            m_items[ItemIndex].Hilited = true;
        }

        public override bool KeyPressed(KeyInfo key)
        {
            if (IsSuspended) return false;

            switch (key.KeyCode)
            {
                case KeyCode.ESCAPE: OnExit(); break;

                case KeyCode.UP: NavigateUp(); break;
                case KeyCode.DOWN: NavigateDown(); break;

                case KeyCode.RETURN: m_items[ItemIndex].Action(); break;

                // stick our false thing here, returning true is the default for handled keys
                default:
                    if (Plugin.Config.GetEnum<InputDevice>(NscConfigKey.ButtonInputDevice) != InputDevice.Keyboard) return false;

                    if (key.KeyCode == Plugin.Config.GetEnum<KeyCode>(NscConfigKey.Key_Back))
                    {
                        OnExit();
                        return true;
                    }

                    if (key.KeyCode == Plugin.Config.GetEnum<KeyCode>(NscConfigKey.Key_BT0))
                    {
                        NavigateUp();
                        return true;
                    }

                    if (key.KeyCode == Plugin.Config.GetEnum<KeyCode>(NscConfigKey.Key_BT1))
                    {
                        NavigateDown();
                        return true;
                    }

                    if (key.KeyCode == Plugin.Config.GetEnum<KeyCode>(NscConfigKey.Key_Start))
                    {
                        m_items[ItemIndex].Action();
                        return true;
                    }

                    return false;
            }

            return true;
        }

        public override bool ButtonPressed(ButtonInfo info)
        {
            if (info.DeviceIndex != Plugin.Gamepad.DeviceIndex) return false;
            if (Plugin.Config.GetEnum<InputDevice>(NscConfigKey.ButtonInputDevice) != InputDevice.Controller) return false;

            if (info.Button == Plugin.Config.GetInt(NscConfigKey.Controller_Back))
            {
                OnExit();
                return true;
            }

            if (info.Button == Plugin.Config.GetInt(NscConfigKey.Controller_BT0))
            {
                NavigateUp();
                return true;
            }

            if (info.Button == Plugin.Config.GetInt(NscConfigKey.Controller_BT1))
            {
                NavigateDown();
                return true;
            }

            if (info.Button == Plugin.Config.GetInt(NscConfigKey.Controller_Start))
            {
                m_items[ItemIndex].Action();
                return true;
            }

            return false;
        }

        protected virtual void OnExit() => Host.PopToParent(this);

        public override void Update(float delta, float total)
        {
            GuiRoot.Update();
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

            DrawUiRoot(GuiRoot);
        }
    }
}
