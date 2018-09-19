using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using theori.Graphics;
using theori.Gui;

namespace theori.Game.States
{
    class VoltexChartSelect_KSH : State
    {
        private Panel foreUiRoot;

        public override void Init()
        {
            foreUiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                }
            };
        }

        public override void Update()
        {
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

            DrawUiRoot(foreUiRoot);
        }
    }
}
