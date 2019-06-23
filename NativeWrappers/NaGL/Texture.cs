using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

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

        public static Texture FromStream2D(Stream stream)
        {
            var texture = new Texture();
            texture.Load2DFromStream(stream);
            return texture;
        }

        public static Texture CreateUninitialized2D() => new Texture(0, TextureTarget.Texture2D);

        static Texture()
        {
            Empty = new Texture();
            Empty.SetEmpty2D(1, 1);
            Empty.Lock();
        }

        public TextureTarget Target = TextureTarget.Texture2D;
        public int Width, Height, Depth;

        private TextureFilter m_minFilter = TextureFilter.Linear;
        private TextureFilter m_magFilter = TextureFilter.Linear;

        public TextureFilter MinFilter { get => m_minFilter; set { m_minFilter = value; SetParams(); } }
        public TextureFilter MagFilter { get => m_magFilter; set { m_magFilter = value; SetParams(); } }

        public void Lock() { Locked = true; }
        public bool Locked { get; private set; }

        private Texture(uint handle, TextureTarget target = TextureTarget.Texture2D)
            : base(handle, GL.DeleteTexture)
        {
            Target = target;
        }

        public Texture(TextureTarget target = TextureTarget.Texture2D)
            : base(GL.GenTexture, GL.DeleteTexture)
        {
            Target = target;
            SetParams();
        }

        public void GenerateHandle()
        {
            if (Handle != 0) return;

            Handle = GL.GenTexture();
            SetParams();
        }

        private void SetParams()
        {
            Bind(0);

            GL.TexParameter((uint)Target, GL.GL_TEXTURE_WRAP_S, GL.GL_CLAMP_TO_EDGE);
            GL.TexParameter((uint)Target, GL.GL_TEXTURE_WRAP_T, GL.GL_CLAMP_TO_EDGE);

            GL.TexParameter((uint)Target, GL.GL_TEXTURE_MIN_FILTER, (uint)MinFilter);
            GL.TexParameter((uint)Target, GL.GL_TEXTURE_MAG_FILTER, (uint)MagFilter);
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

        public void Create2DFromBitmap(Bitmap bmp)
        {
            if (Locked) throw new Exception("Cannot direcly modify a locked texture.");

            Target = TextureTarget.Texture2D;

            Bind(0);
            SetParams();

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

        public void Load2DFromStream(Stream stream)
        {
            using (var bmp = new Bitmap(Image.FromStream(stream)))
                Create2DFromBitmap(bmp);
        }

        public void Load2DFromFile(string fileName)
        {
            using (var bmp = new Bitmap(Image.FromFile(fileName)))
                Create2DFromBitmap(bmp);
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
