using System.Collections.Generic;

namespace System
{
    public struct Variant : IEquatable<Variant>
    {
        public static readonly Variant Null = new Variant(null);

        public static implicit operator Variant(bool v) => new Variant(v);
        
        public static implicit operator Variant(string v) => new Variant(v);
        public static implicit operator Variant(char v) => new Variant(v);
        
        public static implicit operator Variant(sbyte v) => new Variant(v);
        public static implicit operator Variant(short v) => new Variant(v);
        public static implicit operator Variant(int v) => new Variant(v);
        public static implicit operator Variant(long v) => new Variant(v);
        
        public static implicit operator Variant(byte v) => new Variant(v);
        public static implicit operator Variant(ushort v) => new Variant(v);
        public static implicit operator Variant(uint v) => new Variant(v);
        public static implicit operator Variant(ulong v) => new Variant(v);
        
        public static implicit operator Variant(float v) => new Variant(v);
        public static implicit operator Variant(double v) => new Variant(v);
        public static implicit operator Variant(decimal v) => new Variant(v);

        public static implicit operator bool(Variant v) => (bool)v.Value;

        public static implicit operator string(Variant v) => (string)v.Value;
        public static implicit operator char(Variant v) => (char)v.Value;

        public static implicit operator sbyte(Variant v) => (sbyte)v.Value;
        public static implicit operator short(Variant v) => (short)v.Value;
        public static implicit operator int(Variant v) => (int)v.Value;
        public static implicit operator long(Variant v) => (long)v.Value;
        
        public static implicit operator byte(Variant v) => (byte)v.Value;
        public static implicit operator ushort(Variant v) => (ushort)v.Value;
        public static implicit operator uint(Variant v) => (uint)v.Value;
        public static implicit operator ulong(Variant v) => (ulong)v.Value;

        public static implicit operator float(Variant v) => (float)v.Value;
        public static implicit operator double(Variant v) => (double)v.Value;
        public static implicit operator decimal(Variant v) => (decimal)v.Value;
        
        public static bool operator ==(Variant a, Variant b) => EqualityComparer<object>.Default.Equals(a.Value, b.Value);
        public static bool operator !=(Variant a, Variant b) => !(a == b);

        public readonly object Value;

        public Variant(object value)
        {
            Value = value;
        }

        public bool IsNull() => Value == null;
        public bool Is<T>() => Value is T;

        public int ToInt()
        {
            if (Value is IConvertible c)
                return c.ToInt32(Globalization.CultureInfo.CurrentCulture);
            return 0;
        }

        public double ToDouble()
        {
            if (Value is IConvertible c)
                return c.ToDouble(Globalization.CultureInfo.CurrentCulture);
            return 0;
        }

        public bool Equals(Variant v)
        {
            return v.Value == Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Variant v) return v.Value == Value;
            return obj == Value;
        }
        
        public override int GetHashCode() => Value?.GetHashCode() ?? 0;
        public override string ToString() => Value?.ToString() ?? "";
    }
}
