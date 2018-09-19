using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// </summary>
    public readonly struct Ratio : IEquatable<Ratio>, IComparable<Ratio>, IComparable, IFormattable
    {
        #region "Constant"s
        
        public static readonly Ratio Zero = new Ratio(0, 1, false);
        public static readonly Ratio One = new Ratio(1, 1, false);
        public static readonly Ratio MinusOne = new Ratio(-1, 1, false);

        public static readonly Ratio NaN = new Ratio(0, 0, false);
        public static readonly Ratio PositiveInfinity = new Ratio( 1, 0, false);
        public static readonly Ratio NegativeInfinity = new Ratio(-1, 0, false);

        #endregion

        #region Operators
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Ratio((int n, int d) v) => new Ratio(v.n, v.d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Ratio a, Ratio b) =>
            a.Numerator == b.Numerator && a.Denominator == b.Denominator;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Ratio a, Ratio b) =>
            a.Numerator != b.Numerator || a.Denominator != b.Denominator;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Ratio a, Ratio b) =>
            ((long)a.Numerator * b.Denominator) < ((long)a.Denominator * b.Numerator);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Ratio a, Ratio b) =>
            ((long)a.Numerator * b.Denominator) > ((long)a.Denominator * b.Numerator);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Ratio a, Ratio b) =>
            ((long)a.Numerator * b.Denominator) <= ((long)a.Denominator * b.Numerator);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Ratio a, Ratio b) =>
            ((long)a.Numerator * b.Denominator) >= ((long)a.Denominator * b.Numerator);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ratio operator -(Ratio a) => new Ratio(-a.Numerator, a.Denominator, false);

        public static Ratio operator +(Ratio a, Ratio b)
        {
            // if either inputs is invalid
            if (a.Denominator == 0 || b.Denominator == 0)
                // always return NaN
                return NaN;

            int lcm = LCM(a.Denominator, b.Denominator);
            long na = a.Numerator * (long)lcm / a.Denominator;
            long nb = b.Numerator * (long)lcm / b.Denominator;

            long n = na + nb;
            // check for over/underflow, n is the smallest it can be here
            if (n > int.MaxValue)
                return PositiveInfinity;
            else if (n < int.MinValue)
                return NegativeInfinity;

            return new Ratio((int)n, lcm, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ratio operator -(Ratio a, Ratio b) => a + -b;
        
        public static Ratio operator *(Ratio a, Ratio b)
        {
            long n = a.Numerator * (long)b.Numerator;
            long d = a.Denominator * (long)b.Denominator;

            Simplify(ref n, ref d);

            // d will never be negative
            if (d > int.MaxValue)
                // don't worry about creating a "small value" or using infinities
                return NaN;

            // n is big, but d is not
            if (n > int.MaxValue)
                return PositiveInfinity;
            else if (n < int.MinValue)
                return NegativeInfinity;

            return new Ratio((int)n, (int)d, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ratio operator /(Ratio a, Ratio b) => a * ~b;

        public static Ratio operator %(Ratio a, Ratio b)
        {
            long l = a.Numerator * (long)b.Denominator;
            long r = a.Denominator * (long)b.Numerator;

            long n = l - (l / r) * r;
            long d = a.Denominator * (long)b.Denominator;

            Simplify(ref n, ref d);

            // d will never be negative
            if (d > int.MaxValue)
                // don't worry about creating a "small value" or using infinities
                return NaN;

            // n is big, but d is not
            if (n > int.MaxValue)
                return PositiveInfinity;
            else if (n < int.MinValue)
                return NegativeInfinity;

            return new Ratio((int)n, (int)d, false);
        }

        /// <summary>
        /// Returns the reciprocal of this ratio.
        /// </summary>
        public static Ratio operator ~(Ratio a)
        {
            if (a.Denominator == 0)
                return NaN;

            if (a.Numerator > 0)
                return new Ratio(a.Denominator, a.Numerator);
            else if (a.Numerator < 0)
                return new Ratio(-a.Denominator, -a.Numerator);

            return NaN;
        }

        #endregion

        #region Static Functions

        public static Ratio Parse(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (!TryParse(str, out Ratio result))
                throw new FormatException($"{ nameof(str) } is not in a valid ratio format.");

            return result;
        }
        
        public static bool TryParse(string str, out Ratio ratio)
        {
            ratio = NaN;
            if (str == null)
                return false;

            if (str == "+Inf")
            {
                ratio = PositiveInfinity;
                return true;
            }
            
            if (str == "-Inf")
            {
                ratio = NegativeInfinity;
                return true;
            }
            
            if (str == "NaN")
                return true;

            if (str.TrySplit('/', out string numStr, out string denStr))
            {
                if (!int.TryParse(numStr, out int num))
                    return false;
                if (!int.TryParse(denStr, out int den))
                    return false;
                ratio = new Ratio(num, den);
                return true;
            }
            else if (int.TryParse(str, out int num))
            {
                ratio = new Ratio(num, 1);
                return true;
            }
            
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ratio Abs(Ratio a) => new Ratio(Math.Abs(a.Numerator), a.Denominator, false);

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
        /// This will never divide by zero.
        /// This will return 0 if both inputs are 0.
        /// </summary>
        private static long GCD(long v0, long v1)
        {
            while (v1 != 0)
            {
                long temp = v1;
                v1 = v0 % v1;
                v0 = temp;
            }
            return v0;
        }
        
        /// <summary>
        /// LCM will divide by zero when a = 0 and b = 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LCM(int v0, int v1) => v0 / GCD(v0, v1) * v1;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Simplify(ref int n, ref int d)
        {
            int gcd = GCD(n, d);
            if (gcd > 1)
            {
                n /= gcd;
                d /= gcd;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Simplify(ref long n, ref long d)
        {
            long gcd = GCD(n, d);
            if (gcd > 1)
            {
                n /= gcd;
                d /= gcd;
            }
        }

        #endregion

        #region Field's & Constructor

        public readonly int Numerator;
        public readonly int Denominator;

        /// <summary>
        /// Used internally to this struct where simplification isn't necessary.
        /// </summary>
        private Ratio(int n, int d, bool dummy)
        {
            Numerator = n;
            Denominator = d;
        }

        /// <summary>
        /// Simplifies the input Numerator and Denominator.
        /// </summary>
        public Ratio(int n, int d)
        {
            if (n == 0)
            {
                d = 1;
                // guaranteed to already be simplified, skip that step
                goto set_fields;
            }
            // convert to common form for nan, +inf & -inf
            else if (d == 0)
            {
                n = Math.Sign(n);
                // guaranteed to already be simplified, skip that step
                goto set_fields;
            }
            // keep the denominator positive
            else if (d < 0)
            {
                n = -n;
                d = -d;
                // and continue to simplify
            }

            // finally, simplify if needed
            Simplify(ref n, ref d);

        set_fields:
            Numerator = n;
            Denominator = d;
        }

        #endregion

        public void Deconstruct(out int n, out int d)
        {
            n = Numerator;
            d = Denominator;
        }

        #region Equals & GetHashCode

        public override bool Equals(object obj)
        {
            if (obj is Ratio that) return this == that;
            return false;
        }

        public override int GetHashCode() => (Numerator, Denominator).GetHashCode();

        #endregion

        #region Equals

        public bool Equals(Ratio that) => this == that;

        #endregion

        #region Compare

        public int CompareTo(Ratio that)
        {
            return ((long)Numerator * that.Denominator).CompareTo
                   ((long)Denominator * that.Numerator);
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Ratio that) return CompareTo(that);
            throw new ArgumentException($"Cannot compare { typeof(Ratio).Name } to { obj.GetType().Name }.");
        }

        #endregion

        #region ToString

        // Not SUPER usefull, but I left room to add formatting options
        //  and the components already consume them so.

        public override string ToString() => ToString("G", null);
        public string ToString(string format) => ToString(format, null);
        public string ToString(IFormatProvider formatProvider) => ToString(null, formatProvider);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider != null)
            {
                if (formatProvider.GetFormat(typeof(Ratio)) is ICustomFormatter fmt)
                    return fmt.Format(format, this, formatProvider);
            }

            switch (format)
            {
                case "G":
                default:
                {
                    if (Denominator == 0)
                    {
                        if (Numerator == 0)
                            return "NaN";
                        else if (Numerator == 1)
                            return "+Inf";
                        else return "-Inf";
                    }
                    else if (Denominator == 1)
                        return Numerator.ToString(format, formatProvider);
                    else return $"{ Numerator.ToString(format, formatProvider) }/{ Denominator.ToString(format, formatProvider) }";
                }
            }

        }

        #endregion
    }
}
