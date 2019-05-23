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

    public class Font
    {
        private const string FALLBACK_FONT_NAME = "NotoSansCJKjp-Regular.otf";
        private const string FALLBACK_FONT_RESOURCE = "theori." + FALLBACK_FONT_NAME;

        public static readonly Font Default32;
        public static readonly Font Default24;
        public static readonly Font Default16;

        static Font()
        {
            var stream = typeof(Font).Assembly.GetManifestResourceStream(FALLBACK_FONT_RESOURCE);

            byte[] mem = new byte[stream.Length];
            stream.Read(mem, 0, mem.Length);
            stream.Close();

            Default32 = new Font(FALLBACK_FONT_NAME, mem, 32);
            Default24 = new Font(FALLBACK_FONT_NAME, mem, 24);
            Default16 = new Font(FALLBACK_FONT_NAME, mem, 16);
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
                        for (int j = 0; j < 4; j++)
                            pixelData[i * 4 + j] = bufferData[i];
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
