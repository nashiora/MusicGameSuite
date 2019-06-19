using System;
using System.Numerics;

using theori.IO;

namespace theori.Gui
{
    public class Button : Panel
    {
        private Sprite image;

        public Action Pressed;

        public Button()
        {
            Children = new GuiElement[]
            {
                image = new Sprite(OpenGL.Texture.Empty)
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = Vector2.One,
                },
            };
        }

        public override void Update()
        {
            base.Update();

            if (ContainsScreenPoint(Mouse.Position))
                image.Color = new Vector4(1, 1, 0, 1);
            else image.Color = Vector4.One;
        }

        public override bool OnMouseButtonPress(MouseButton button)
        {
            Pressed?.Invoke();
            return true;
        }
    }
}
