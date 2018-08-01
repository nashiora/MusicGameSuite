using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxMania
{
    /// <summary>
    /// Left and Top mean negative direction, Right and Bottom mean positive.
    /// </summary>
    [Flags]
    public enum Anchor
    {
        Top = 0x01,
        VerticalCenter = 0x02,
        Bottom = 0x04,

        Left = 0x08,
        HorizontalCenter = 0x10,
        Right = 0x20,

        TopLeft = Top | Left,
        TopCenter = Top | HorizontalCenter,
        TopRight = Top | Right,
        
        CenterLeft = VerticalCenter | Left,
        Center = VerticalCenter | HorizontalCenter,
        CenterRight = VerticalCenter | Right,
        
        BottomLeft = Bottom | Left,
        BottomCenter = Bottom | HorizontalCenter,
        BottomRight = Bottom | Right,
    }
}
