using System;
using System.Globalization;

namespace theori
{
    public readonly struct tick_t : IEquatable<tick_t>, IComparable, IComparable<tick_t>, IFormattable
    {
        #region Static Constructors

        public static tick_t FromFloat(double v) => new tick_t(v);
        public static tick_t FromFraction(int num, int denom) => new tick_t(num, denom);

        #endregion

        #region Numeric Conversions

        public static implicit operator tick_t(double v) => new tick_t(v);
        
        public static explicit operator double(tick_t t) => t.m_value;

        #endregion

        #region Core Data

        private readonly double m_value;

        private tick_t(double value)
        {
            m_value = value;
        }

        public tick_t(int num, int denom)
        {
            m_value = (double)num / denom;
        }

        #endregion

        #region Relational Operators
        
        public static bool operator ==(tick_t a, tick_t b) =>  a.Equals(b);
        public static bool operator !=(tick_t a, tick_t b) => !a.Equals(b);
        
        public static bool operator < (tick_t a, tick_t b) => a.m_value <  b.m_value;
        public static bool operator > (tick_t a, tick_t b) => a.m_value >  b.m_value;
        public static bool operator <=(tick_t a, tick_t b) => a.m_value <= b.m_value;
        public static bool operator >=(tick_t a, tick_t b) => a.m_value >= b.m_value;

        #endregion

        #region Arithmetic Operators
        
        public static tick_t operator +(tick_t a, tick_t b) => new tick_t(a.m_value + b.m_value);
        public static tick_t operator -(tick_t a, tick_t b) => new tick_t(a.m_value - b.m_value);
        
        public static tick_t operator *(tick_t a, double factor) => new tick_t(a.m_value * factor);
        public static tick_t operator *(tick_t a, int factor) => new tick_t(a.m_value * factor);
        
        public static tick_t operator *(double factor, tick_t a) => new tick_t(a.m_value * factor);
        public static tick_t operator *(int factor, tick_t a) => new tick_t(a.m_value * factor);
        
        public static time_t operator *(time_t time, tick_t tick) => time * tick.m_value;
        public static time_t operator *(tick_t tick, time_t time) => time * tick.m_value;
        
        public static tick_t operator /(tick_t a, double factor) => new tick_t(a.m_value / factor);
        public static tick_t operator /(tick_t a, int factor) => new tick_t(a.m_value / factor);

        public static tick_t operator %(tick_t a, tick_t b) => new tick_t(a.m_value % b.m_value);

        #endregion

        #region IComparable

        public int CompareTo(tick_t other) => m_value.CompareTo(other.m_value);
        public int CompareTo(object obj)
        {
            if (obj is tick_t other)
                return CompareTo(other);
            throw new ArgumentException($"Attempt to compare { obj?.GetType()?.FullName ?? "null" } to { typeof(tick_t).FullName }");
        }

        #endregion

        #region IEquatable

        public bool Equals(tick_t other) => m_value == other.m_value;
        public override bool Equals(object obj)
        {
            if (obj is tick_t other)
                return Equals(other);
            return false;
        }

        #endregion
        
        public override int GetHashCode() => m_value.GetHashCode();

        public void ToFraction(out int num, ref int denom)
        {
            double value = m_value;
            
            bool neg = value < 0;
            if (neg) value = -value;

            num = (int)Math.Round(value * denom);
            Simplify(ref num, ref denom);

            if (neg) num = -num;
        }

        public void ToFraction(int maxDenom, out int num, out int denom)
        {
            denom = maxDenom;
            ToFraction(out num, ref denom);
        }

        #region IFormattable

        public override string ToString() => ToString("G", null);
        public string ToString(string format) => ToString(format, null);
        public string ToString(IFormatProvider provider) => ToString("G", provider);
        
        /// <summary>
        /// Format options:
        /// 
        /// - "FI" to format as an improper fraction.
        /// 
        /// - "FW" or "F" to format as a proper fraction (having a `W`hole value).
        ///   
        /// - "G" or any invalid string defaults to "D".
        /// 
        /// The given format provider is passed to the numeric value.
        /// </summary>
        public string ToString(string format, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(format))
                format = "G";
            format = format.Trim().ToUpperInvariant();

            if (provider == null)
                provider = NumberFormatInfo.CurrentInfo;

            switch (format)
            {
                case "G": default:
                case "D": return $"{ m_value.ToString(provider) } Tx";
                    
                case "F":
                case "FW":
                {
                    //int num, denom = Chart.UnitResolution;
                    int num, denom = 192;
                    ToFraction(out num, ref denom);

                    int sign = Math.Sign(num);
                    int numAbs = Math.Abs(num);

                    int whole = numAbs / denom;
                    num = numAbs - whole * denom;

                    if (whole != 0)
                        whole *= sign;
                    else num *= sign;

                    if (whole != 0)
                        return $"{ whole.ToString(provider) } { num.ToString(provider) }/{ denom.ToString(provider) } Tx";
                    else return $"{ num.ToString(provider) }/{ denom.ToString(provider) } Tx";
                }
                    
                case "FI":
                {
                    //int num, denom = Chart.UnitResolution;
                    int num, denom = 192;
                    ToFraction(out num, ref denom);

                    return $"{ num.ToString(provider) }/{ denom.ToString(provider) } Tx";
                }
            }
        }

        /// <summary>
        /// This will never divide by zero.
        /// This will return 0 if both inputs are 0.
        /// </summary>
        private static int GCD(int v0, int v1)
        {
            while (v1 != 0)
            {
                int temp = v1;
                v1 = v0 % v1;
                v0 = temp;
            }
            return v0;
        }
        
        /// <summary>
        /// LCM will divide by zero when a = 0 and b = 0.
        /// </summary>
        private static int LCM(int v0, int v1) => v0 / GCD(v0, v1) * v1;
        
        private static void Simplify(ref int n, ref int d)
        {
            int gcd = GCD(n, d);
            if (gcd > 1)
            {
                n /= gcd;
                d /= gcd;
            }
        }

        #endregion
    }
}
