using System;

namespace theori
{
    /// <summary>
    /// Left and Top mean negative direction, Right and Bottom mean positive.
    /// </summary>
    [Flags]
    public enum Anchor
    {
        Top = 0x01,
        Middle = 0x02,
        Bottom = 0x04,

        Left = 0x10,
        Center = 0x20,
        Right = 0x40,

        TopLeft = Top | Left,
        TopCenter = Top | Center,
        TopRight = Top | Right,
        
        MiddleLeft = Middle | Left,
        MiddleCenter = Middle | Center,
        MiddleRight = Middle | Right,
        
        BottomLeft = Bottom | Left,
        BottomCenter = Bottom | Center,
        BottomRight = Bottom | Right,
    }
}
