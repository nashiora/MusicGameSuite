using System.Numerics;

using OpenGL;

using theori.Gui;
using theori.Graphics;
using System;

namespace theori.Game
{
    public sealed class CriticalLine : Panel
    {
        private bool m_isDirty = true;

        private readonly Panel m_container;
        private readonly Sprite m_image, m_capLeft, m_capRight;

        private float m_horHeight, m_critHeight;
        private float m_laserRoll, m_addRoll, m_addOffset;

        public float HorizonHeight { get => m_horHeight; set { m_horHeight = value; m_isDirty = true; } }
        public float CriticalHeight { get => m_critHeight; set { m_critHeight = value; m_isDirty = true; } }
        
        public float LaserRoll { get => m_laserRoll; set { m_laserRoll = value; m_isDirty = true; } }
        public float EffectRoll { get => m_addRoll; set { m_addRoll = value; m_isDirty = true; } }
        public float EffectOffset { get => m_addOffset; set { m_addOffset = value; m_isDirty = true; } }

        public CriticalLine()
        {
            var critTexture = new Texture();
            critTexture.Load2DFromFile(@".\skins\Default\textures\scorebar.png");
            
            var capTexture = new Texture();
            capTexture.Load2DFromFile(@".\skins\Default\textures\critical_cap.png");

            RelativeSizeAxes = Axes.X;
            Size = new Vector2(0.75f, 0);

            Children = new GuiElement[]
            {
                m_container = new Panel()
                {
                    RelativeSizeAxes = Axes.X,
                    Size = new Vector2(1, 0),

                    Children = new GuiElement[]
                    {
                        m_image = new Sprite(critTexture),
                        m_capLeft = new Sprite(capTexture)
                        {
                            Position = new Vector2(20, 0),
                        },
                        m_capRight = new Sprite(capTexture)
                        {
                            Scale = new Vector2(-1, 1),
                        },
                    }
                },

                new Sprite(null)
                {
                    Size = new Vector2(1, 100),
                    Color = new Vector4(1, 1, 0, 1),
                }
            };

            float critImageWidth = m_image.Size.X;

            m_container.Size = new Vector2(critImageWidth, 0);
            m_container.Origin = m_container.Size / 2;
            
            m_image.Origin = new Vector2(0, m_image.Size.Y / 2);

            m_capLeft.Origin = new Vector2(m_capLeft.Size.X, m_capLeft.Size.Y / 2);

            m_capRight.Position = new Vector2(critImageWidth - 20, 0);
            m_capRight.Origin = new Vector2(m_capRight.Size.X, m_capRight.Size.Y / 2);
        }

        public override void Update()
        {
            if (m_isDirty)
            {
                UpdateOrientation();
                m_isDirty = false;
            }
        }

        private void UpdateOrientation()
        {
            Position = new Vector2(Window.Width / 2, HorizonHeight);
            Rotation = -LaserRoll - MathL.Sin(EffectRoll * MathL.Pi * 2) * 30;

            float critDist = CriticalHeight - HorizonHeight;
            float desiredCritWidth = Window.Width * 0.75f;

            m_container.Position = new Vector2(-LaserRoll * 10 + EffectOffset * 100, critDist);
            m_container.Scale = new Vector2(desiredCritWidth / m_image.Size.X);
        }
    }
}
