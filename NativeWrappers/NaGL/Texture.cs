using System;
using System.Drawing;

namespace OpenGL
{
    public sealed class Texture : UIntHandle
    {
        public static readonly Texture Empty;

        public static Texture FromFile2D(string fileName)
        {
            var texture = new Texture();
            texture.Load2DFromFile(fileName);
            return texture;
        }

        static Texture()
        {
            Empty = new Texture();
            Empty.SetEmpty2D(1, 1);
            Empty.Lock();
        }

        public TextureTarget Target = TextureTarget.Texture2D;
        public int Width, Height, Depth;
        
        public void Lock() { Locked = true; }
        public bool Locked { get; private set; }

        public Texture(TextureTarget target = TextureTarget.Texture2D)
            : base(GL.GenTexture, GL.DeleteTexture)
        {
            Target = target;
            SetParams();
        }

        private void SetParams()
        {
            Bind(0);

            GL.TexParameter((uint)Target, GL.GL_TEXTURE_WRAP_S, GL.GL_CLAMP);
            GL.TexParameter((uint)Target, GL.GL_TEXTURE_WRAP_T, GL.GL_CLAMP);

            GL.TexParameter((uint)Target, GL.GL_TEXTURE_MIN_FILTER, GL.GL_NEAREST);
            GL.TexParameter((uint)Target, GL.GL_TEXTURE_MAG_FILTER, GL.GL_NEAREST);
        }

        public void Bind(uint unit)
        {
            if (false && GL.IsExtensionFunctionSupported("glBindTextureUnit"))
                GL.BindTextureUnit(unit, Handle);
            else
            {
			    GL.ActiveTexture(GL.GL_TEXTURE0 + unit);
                GL.BindTexture((uint)Target, Handle);
            }
        }

        public void SetEmpty2D(int width, int height)
        {
            if (Locked) throw new Exception("Cannot direcly modify a locked texture.");

            Target = TextureTarget.Texture2D;

            Bind(0);
            SetParams();

            Width = width;
            Height = height;
            Depth = 0;
            
            byte[] pixels = new byte[Width * Height * 4];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = byte.MaxValue;

            GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, Width, Height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixels);
        }

        public void Load2DFromFile(string fileName)
        {
            if (Locked) throw new Exception("Cannot direcly modify a locked texture.");

            Target = TextureTarget.Texture2D;

            Bind(0);
            SetParams();

            using (var bmp = new Bitmap(Image.FromFile(fileName)))
            {
                Width = bmp.Width;
                Height = bmp.Height;
                Depth = 0;

                byte[] pixels = new byte[Width * Height * 4];
                for (int i = 0; i < Width * Height; i++)
                {
                    int x = i % bmp.Width;
                    int y = i / bmp.Width;

                    var p = bmp.GetPixel(x, y);
                
                    pixels[0 + i * 4] = p.R;
                    pixels[1 + i * 4] = p.G;
                    pixels[2 + i * 4] = p.B;
                    pixels[3 + i * 4] = p.A;
                }

                GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, Width, Height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixels);
            }
        }

        public void SetData2D(int width, int height, byte[] pixelData)
        {
            if (Locked) throw new Exception("Cannot direcly modify a locked texture.");

            Target = TextureTarget.Texture2D;

            Bind(0);
            SetParams();

            Width = width;
            Height = height;
            Depth = 0;

            GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, Width, Height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixelData);
        }
    }
}
