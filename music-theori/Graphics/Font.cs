using System;
using System.Collections.Generic;
using System.Numerics;

using OpenGL;

using SharpFont;

namespace theori.Graphics
{
    public struct GlyphInfo
    {
        public uint GlyphIndex;

        public int BitmapLeft;

        public Vector2 Advance;
        public GlyphMetrics Metrics;
        public Vector2 Kerning;
    }

    public struct GlyphMetrics
    {
        public float Width, Height;
        public float HorizontalBearingX, HorizontalBearingY;
    }

    /// <summary>
    /// Calling GetDefault results in repeated opening, copying and closing of a stream of
    ///  the file font unless the specific given font size has already been created and cached.
    /// This is because I haven't figured out how I want to go about pixel size.
    /// The PixelSize property is treated effectively read only, despite the option clearly
    ///  being there to change it.
    /// It shouldn't be hard to specify the texture size before rendering glyphs, and cache glyphs
    ///  based on what size was used to render them, ut just hasn't been done yet.
    /// The "Default" API will need re-adjusted soon when that kind of change is implemented.
    /// </summary>
    public class Font
    {
        private const string FALLBACK_FONT_NAME = "fonts/NotoSansCJKjp-Regular.otf";

        private static readonly Dictionary<uint, Font> m_defaults = new Dictionary<uint, Font>();

        public static Font Default64 => m_defaults[64];
        public static Font Default32 => m_defaults[32];
        public static Font Default24 => m_defaults[24];
        public static Font Default16 => m_defaults[16];
        public static Font Default12 => m_defaults[12];
        public static Font Default8  => m_defaults[ 8];

        static Font()
        {
            using (var stream = Resources.ClientResourceLocator.Default.OpenFileStream(FALLBACK_FONT_NAME))
            {
                byte[] mem = new byte[stream.Length];
                stream.Read(mem, 0, mem.Length);

                uint[] defaultSize = { 8, 12, 16, 24, 32, 64 };
                foreach (uint size in defaultSize)
                    m_defaults[size] = new Font(FALLBACK_FONT_NAME, mem, size);
            }
        }

        public static Font GetDefault(int size)
        {
            uint usize = (uint)MathL.Clamp(size, 7, 256);
            if (!m_defaults.TryGetValue(usize, out var font))
            {
                using (var stream = Resources.ClientResourceLocator.Default.OpenFileStream(FALLBACK_FONT_NAME))
                {
                    byte[] mem = new byte[stream.Length];
                    stream.Read(mem, 0, mem.Length);

                    font = new Font(FALLBACK_FONT_NAME, mem, usize);
                    m_defaults[usize] = font;
                }
            }
            return font;
        }

        public string Name { get; }

        private readonly Library lib;
        private readonly Face face;

        public bool HasKerning => face.HasKerning;

        private uint currentGlyph;

        private readonly Dictionary<uint, Texture> texCache = new Dictionary<uint, Texture>();

        private uint pixelSize;
        public uint PixelSize
        {
            get => pixelSize;
            set => face.SetPixelSizes(0, pixelSize = value);
        }

        public int LineSpacing => (int)PixelSize;

        private Font(string fontName, byte[] fontMemory, uint pixelSize)
        {
            Name = fontName;

            lib = new Library();
            face = new Face(lib, fontMemory, 0);

            PixelSize = pixelSize;
        }

        public Font(string fontName, uint pixelSize)
        {
            Name = fontName;

            lib = new Library();
            face = new Face(lib, fontName);

            PixelSize = pixelSize;
        }

        public uint GetCharIndex(char c) => face.GetCharIndex(c);

        public void LoadGlyph(uint glyphIndex) => face.LoadGlyph(currentGlyph = glyphIndex, LoadFlags.Default, LoadTarget.Normal);
        public GlyphInfo GetGlyphInfo()
        {
            return new GlyphInfo()
            {
                BitmapLeft = face.Glyph.BitmapLeft,

                Advance = new Vector2((float)face.Glyph.Advance.X, (float)face.Glyph.Advance.Y),

                Metrics = new GlyphMetrics()
                {
                    Width = (float)face.Glyph.Metrics.Width,
                    Height = (float)face.Glyph.Metrics.Height,
                    HorizontalBearingX = (float)face.Glyph.Metrics.HorizontalBearingX,
                    HorizontalBearingY = (float)face.Glyph.Metrics.HorizontalBearingY,
                },
            };
        }

        public Vector2 GetKerning(uint left, uint right)
        {
            var result = face.GetKerning(left, right, KerningMode.Default);
            return new Vector2((float)result.X, (float)result.Y);
        }

        public FTBitmap RenderCurrentGlyph()
        {
            face.Glyph.RenderGlyph(RenderMode.Normal);
            return face.Glyph.Bitmap;
        }

        public Texture RasterizeGlyph(uint glyphIndex)
        {
            LoadGlyph(glyphIndex);
            return RasterizeCurrentGlyph();
        }

        public Texture RasterizeCurrentGlyph()
        {
            if (texCache.TryGetValue(currentGlyph, out var texture))
                return texture;

            var bitmap = RenderCurrentGlyph();

            texture = new Texture();
            switch (bitmap.PixelMode)
            {
                case PixelMode.Gray:
                {
                    byte[] bufferData = bitmap.BufferData;
                    byte[] pixelData = new byte[4 * bufferData.Length];

                    for (int i = 0; i < bufferData.Length; i++)
                    {
                        for (int j = 0; j < 3; j++)
                            pixelData[i * 4 + j] = 255;
                        pixelData[i * 4 + 3] = bufferData[i];
                    }

                    texture.SetData2D(bitmap.Width, bitmap.Rows, pixelData);
                } break;

                default: throw new NotImplementedException(bitmap.PixelMode.ToString());
            }

            texCache[currentGlyph] = texture;
            return texture;
        }

        public Vector2 MeasureString(string text)
        {
            float stringWidth = 0;
            float stringHeight = 0;
            float overrun = 0;
            float underrun = 0;
            float kern = 0;

            int spacingError = 0;
            bool trackingUnderrun = true;
            int rightEdge = 0;

            float top = 0, bottom = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                uint glyphIndex = GetCharIndex(c);

                LoadGlyph(glyphIndex);
                var info = GetGlyphInfo();

                float gAdvanceX = info.Advance.X;
                float gBearingX = info.Metrics.HorizontalBearingX;
                float gWidth = info.Metrics.Width;

                underrun += -gBearingX;
                if (stringWidth == 0)
                    stringWidth += underrun;
                if (trackingUnderrun && underrun <= 0)
                {
                    underrun = 0;
                    trackingUnderrun = false;
                }

                if (gBearingX + gWidth > 0 || gAdvanceX > 0)
                {
                    overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
                    if (overrun <= 0) overrun = 0;
                }

                overrun += gBearingX == 0 && gWidth == 0 ? 0 : gBearingX + gWidth - gAdvanceX;
                if (i == text.Length - 1)
                    stringWidth += overrun;

                float gTop = info.Metrics.HorizontalBearingY;
                float gBottom = info.Metrics.Height - info.Metrics.HorizontalBearingY;

                if (gTop > top)
                    top = gTop;
                if (gBottom > bottom)
                    bottom = gBottom;

                stringWidth += gAdvanceX;

                if (HasKerning && i < text.Length - 1)
                {
                    char cNext = text[i + 1];
                    kern = GetKerning(glyphIndex, GetCharIndex(cNext)).X;
                    if (kern > gAdvanceX * 5 || kern < -(gAdvanceX * 5))
                        kern = 0;
                    stringWidth += kern;
                }
            }

            stringHeight = top + bottom;

            return new Vector2(stringWidth, stringHeight);
        }
    }
}
