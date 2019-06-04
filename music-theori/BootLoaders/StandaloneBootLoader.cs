using System.Numerics;

using theori.Graphics;
using theori.Gui;

namespace theori.BootLoaders
{
    public sealed class StandaloneBootLoader : Layer
    {
        private Panel m_guiRoot;

        public StandaloneBootLoader(string[] args)
        {
        }

        public override void Init()
        {
            m_guiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                    new TextLabel(Font.Default24, "music:theori Standalone Boot Loader")
                    {
                        Position = new Vector2(20, 20),
                    },
                }
            };
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
