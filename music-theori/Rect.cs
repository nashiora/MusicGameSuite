using System;
using System.Numerics;

namespace theori
{
    public struct Rect : IEquatable<Rect>
    {
        public static Rect EmptyScissor => new Rect(Vector2.Zero, -Vector2.One);

        public Vector2 Position;
        public Vector2 Size;

        public float Left
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }

        public float Right
        {
            get => Position.X + Size.X;
            set => Size = new Vector2(value - Position.X, Size.Y);
        }

        public float Top
        {
            get => Position.Y;
            set => Position = new Vector2(Position.X, value);
        }

        public float Bottom
        {
            get => Position.Y + Size.Y;
            set => Size = new Vector2(Size.X, value - Position.Y);
        }

        public float Width
        {
            get => Size.X;
            set => Size = new Vector2(value, Size.Y);
        }

        public float Height
        {
            get => Size.Y;
            set => Size = new Vector2(Size.X, value);
        }

        public Rect(Vector2 pos, Vector2 size)
        {
            Position = pos;
            Size = size;
        }

        public Rect(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public Rect Clamp(Rect that)
        {
		    float top = Math.Max(that.Top, Top);
		    float bottom = Math.Min(that.Bottom, Bottom);
		    float left = Math.Max(that.Left, Left);
		    float right = Math.Min(that.Right, Right);
             
		    if(right < left) right = left;
		    if(bottom < top) bottom = top;
            
		    return new Rect(left, top, right, bottom);
        }

        public override bool Equals(object obj)
        {
            if (obj is Rect that) return Equals(that);
            return false;
        }

        public bool Equals(Rect that) => Position == that.Position && Size == that.Size;

        public override int GetHashCode() => Position.GetHashCode() ^ (27 * Size.GetHashCode());
    }
}
