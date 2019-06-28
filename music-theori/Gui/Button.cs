using System;
using System.Numerics;

using theori.IO;

namespace theori.Gui
{
    public class Button : Panel
    {
        private readonly Sprite m_image;

        public Action Pressed;

        public Button()
        {
            Children = new GuiElement[]
            {
                m_image = new Sprite(OpenGL.Texture.Empty)
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
                m_image.Color = new Vector4(1, 1, 0, 1);
            else m_image.Color = Vector4.One;
        }

        public override bool OnMouseButtonPress(MouseButton button)
        {
            Pressed?.Invoke();
            return true;
        }
    }
}
