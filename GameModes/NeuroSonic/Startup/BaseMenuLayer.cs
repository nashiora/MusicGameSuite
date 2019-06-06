using System;
using System.Collections.Generic;
using System.Numerics;

using theori;
using theori.Graphics;
using theori.Gui;

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
        private Panel m_guiRoot;

        private int m_itemIndex;
        private readonly List<MenuItem> m_items = new List<MenuItem>();

        private int m_extraSpacing = 0;

        protected int ItemIndex => m_items.Count + m_extraSpacing;

        protected abstract string Title { get; }

        public override void Init()
        {
            GenerateMenuItems();

            m_guiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                    new TextLabel(Font.Default32, Title)
                    {
                        Position = new Vector2(20, 20),
                    },

                    new Panel()
                    {
                        Position = new Vector2(40, 100),

                        Children = m_items
                    },
                }
            };

            m_items[m_itemIndex].Hilited = true;
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

        public override bool KeyPressed(KeyInfo key)
        {
            if (IsSuspended) return false;

            switch (key.KeyCode)
            {
                case KeyCode.ESCAPE: OnExit(); break;

                case KeyCode.UP:
                {
                    m_items[m_itemIndex].Hilited = false;
                    m_itemIndex = Math.Max(0, m_itemIndex - 1);
                    m_items[m_itemIndex].Hilited = true;
                }
                break;

                case KeyCode.DOWN:
                {
                    m_items[m_itemIndex].Hilited = false;
                    m_itemIndex = Math.Min(m_items.Count - 1, m_itemIndex + 1);
                    m_items[m_itemIndex].Hilited = true;
                }
                break;

                case KeyCode.RETURN: m_items[m_itemIndex].Action(); break;

                // stick our false thing here, returning true is the default for handled keys
                default: return false;
            }

            return true;
        }

        protected virtual void OnExit() => Host.PopToParent(this);

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
