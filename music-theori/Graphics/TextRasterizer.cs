using System;
using System.Collections.Generic;
using System.Diagnostics;

using OpenGL;

namespace theori.Graphics
{
    public class TextRasterizer : IDisposable
    {
        public bool IsDirty { get; private set; } = true;

        private string m_text;
        private Font m_font;

        private Texture m_texture;
        public Texture Texture
        {
            get
            {
                if (IsDirty)
                    Rasterize();
                return m_texture;
            }
        }

        public string Text
        {
            get => m_text;
            set
            {
                if (value == m_text)
                    return;

                m_text = value;
                IsDirty = true;
            }
        }

        public Font Font
        {
            get => m_font;
            set
            {
                if (value == m_font)
                    return;

                m_font = value;
                IsDirty = true;
            }
        }

        public float BaseLine { get; private set; }

        public int Width => m_texture?.Width ?? 0;
        public int Height => m_texture?.Height ?? 0;

        public TextRasterizer(Font font, string text)
        {
            m_font = font;
            m_text = text;
        }

        public void Dispose()
        {
            m_texture?.Dispose();
            m_texture = null;
        }

        public void Rasterize()
        {
            if (Text == null)
                return;

            var measuredChars = new List<DebugChar>();
            var renderedChars = new List<DebugChar>();

            string text = Text;
            var font = Font;

            float penX = 0, penY = 0;
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
                uint glyphIndex = Font.GetCharIndex(c);

                Font.LoadGlyph(glyphIndex);
                var info = Font.GetGlyphInfo();

                float gAdvanceX = info.Advance.X;
                float gBearingX = info.Metrics.HorizontalBearingX;
                float gWidth = info.Metrics.Width;

                var rc = new DebugChar(c, gAdvanceX, gBearingX, gWidth);

                underrun += -gBearingX;
                if (stringWidth == 0)
                    stringWidth += underrun;
                if (trackingUnderrun)
                    rc.Underrun = underrun;
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
                rc.Overrun = overrun;

                float gTop = info.Metrics.HorizontalBearingY;
                float gBottom = info.Metrics.Height - info.Metrics.HorizontalBearingY;

                if (gTop > top)
                    top = gTop;
                if (gBottom > bottom)
                    bottom = gBottom;

                stringWidth += gAdvanceX;
                rc.RightEdge = stringWidth;
                measuredChars.Add(rc);

                if (Font.HasKerning && i < text.Length - 1)
                {
                    char cNext = text[i + 1];
                    kern = Font.GetKerning(glyphIndex, Font.GetCharIndex(cNext)).X;
                    if (kern > gAdvanceX * 5 || kern < -(gAdvanceX * 5))
                        kern = 0;
                    rc.Kern = kern;
                    stringWidth += kern;
                }
            }

            stringHeight = top + bottom;
            BaseLine = top;

            if (stringWidth == 0 || stringHeight == 0)
            {
                IsDirty = false;
                return;
            }

            m_texture?.Dispose();
            m_texture = new Texture();

            int textureWidth = (int)Math.Ceiling(stringWidth + 2), textureHeight = (int)Math.Ceiling(stringHeight);

            var pixelData = new byte[4 * textureWidth * textureHeight];
            pixelData.Fill((byte)0);

            void SetPixelSubData(Rect bounds, byte[] bufferData)
            {
                int boundsX = (int)bounds.Left, boundsY = (int)bounds.Top;
                int bufferWidth = (int)bounds.Width, bufferHeight = (int)bounds.Height;

                for (int y = 0; y < bufferHeight; y++)
                {
                    for (int x = 0; x < bufferWidth; x++)
                    {
                        int bufferDataIndex = y * bufferWidth + x;
                        int pixelDataIndex = 4 * ((y + boundsY) * textureWidth + (x + boundsX));

                        if (pixelDataIndex < 0) continue;
                        for (int j = 0; j < 3; j++)
                            pixelData[pixelDataIndex + j] = 255;
                        pixelData[pixelDataIndex + 3] = bufferData[bufferDataIndex];
                    }
                }
            }

            trackingUnderrun = true;

            underrun = 0;
            overrun = 0;
            stringWidth = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                uint glyphIndex = Font.GetCharIndex(c);

                Font.LoadGlyph(glyphIndex);
                var bitmap = Font.RenderCurrentGlyph();

                var info = Font.GetGlyphInfo();

                float gAdvanceX = info.Advance.X;
                float gBearingX = info.Metrics.HorizontalBearingX;
                float gWidth = info.Metrics.Width;

                var rc = new DebugChar(c, gAdvanceX, gBearingX, gWidth);

                underrun += -gBearingX;
                if (penX == 0)
                    penX += underrun;
                if (trackingUnderrun)
                    rc.Underrun = underrun;
                if (trackingUnderrun && underrun <= 0)
                {
                    underrun = 0;
                    trackingUnderrun = false;
                }

                if (bitmap.Width > 0 && bitmap.Rows > 0)
                {
                    rc.Width = bitmap.Width;
                    rc.BearingX = info.BitmapLeft;

                    int x = (int)Math.Round(penX + info.BitmapLeft);
                    int y = (int)Math.Round(penY + top - info.Metrics.HorizontalBearingY);

                    var bounds = new Rect(x, y, bitmap.Width, bitmap.Rows);

                    switch (bitmap.PixelMode)
                    {
                        case SharpFont.PixelMode.Gray:
                            SetPixelSubData(bounds, bitmap.BufferData);
                            break;

                        default: throw new NotImplementedException(bitmap.PixelMode.ToString());
                    }

                    rc.Overrun = info.BitmapLeft + bitmap.Width - gAdvanceX;

                    rightEdge = Math.Max(rightEdge, x + bitmap.Width);
                    spacingError = m_texture.Width - rightEdge;
                }
                else
                {
                    rightEdge = (int)(penX + gAdvanceX);
                    spacingError = m_texture.Width - rightEdge;
                }

                if (gBearingX + gWidth > 0 || gAdvanceX > 0)
                {
                    overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
                    if (overrun <= 0) overrun = 0;
                }

                overrun += gBearingX == 0 && gWidth == 0 ? 0 : gBearingX + gWidth - gAdvanceX;
                if (i == text.Length - 1)
                    penX += overrun;
                rc.Overrun = overrun;

                penX += info.Advance.X;
                penY += info.Advance.Y;
                renderedChars.Add(rc);

                if (Font.HasKerning && i < text.Length - 1)
                {
                    char cNext = text[i + 1];
                    kern = Font.GetKerning(glyphIndex, Font.GetCharIndex(cNext)).X;
                    if (kern > gAdvanceX * 5 || kern < -(gAdvanceX * 5))
                        kern = 0;
                    rc.Kern = kern;
                    penX += kern;
                }
            }

            m_texture.SetData2D(textureWidth, textureHeight, pixelData);
            IsDirty = false;
        }

        private class DebugChar
        {
            public char Char { get; set; }
            public float AdvanceX { get; set; }
            public float BearingX { get; set; }
            public float Width { get; set; }
            public float Underrun { get; set; }
            public float Overrun { get; set; }
            public float Kern { get; set; }
            public float RightEdge { get; set; }

            internal DebugChar(char c, float advanceX, float bearingX, float width)
            {
                Char = c;
                AdvanceX = advanceX;
                BearingX = bearingX;
                Width = width;
            }

            public override string ToString()
            {
                return string.Format("'{0}' {1,5:F0} {2,5:F0} {3,5:F0} {4,5:F0} {5,5:F0} {6,5:F0} {7,5:F0}",
                    Char, AdvanceX, BearingX, Width, Underrun, Overrun, Kern, RightEdge);
            }

            public static void PrintHeader()
            {
                Debug.Print("    {1,5} {2,5} {3,5} {4,5} {5,5} {6,5} {7,5}", "", "adv", "bearing", "wid", "undrn", "ovrrn", "kern", "redge");
            }
        }
    }
}
