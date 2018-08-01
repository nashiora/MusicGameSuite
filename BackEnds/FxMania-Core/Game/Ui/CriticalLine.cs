using OpenGL;

using FxMania.Gui;

namespace FxMania.Game.Ui
{
    public class CriticalLine : Panel
    {
        private Panel critContainer;
        private Sprite critImage, capLeft, capRight;

        public CriticalLine()
        {
            var critTexture = new Texture();
            critTexture.Load2DFromFile(@".\skins\Default\textures\scorebar.png");
            
            var capTexture = new Texture();
            capTexture.Load2DFromFile(@".\skins\Default\textures\critical_cap.png");

            Children = new GuiElement[]
            {
                critContainer = new Panel()
                {
                    Children = new GuiElement[]
                    {
                        critImage = new Sprite(critTexture),
                        capLeft = new Sprite(capTexture),
                        capRight = new Sprite(capTexture),
                    }
                },
            };
        }
    }
}
