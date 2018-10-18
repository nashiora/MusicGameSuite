using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenRM.Convert;
using theori.Audio;
using theori.Graphics;
using theori.Gui;
using theori.Platform;

namespace theori.Game.Scenes
{
    class VoltexChartSelect_KSH : Scene
    {
        private GuiManager uiManager;
        private Panel foreUiRoot;
        private Button openButton;

        public override void Init()
        {
            foreUiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                    openButton = new Button()
                    {
                        Origin = new Vector2(100, 25),

                        Size = new Vector2(200, 50),

                        RelativePositionAxes = Axes.X,
                        Position = new Vector2(0.5f, 50),
                    },
                }
            };

            uiManager = new GuiManager(foreUiRoot);

            Keyboard.KeyPress += Keyboard_KeyPress;
        }

        private void Keyboard_KeyPress(KeyInfo key)
        {
            if (key.KeyCode == KeyCode.O)
            {
            }
        }

        public override void Update()
        {
            foreUiRoot.Update();
            uiManager.Update();
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
