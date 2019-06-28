using System;
using System.Globalization;

namespace theori
{
    /// <summary>
    /// Deals with telling time in terms of seconds, or variations of seconds.
    /// </summary>
    public readonly struct time_t : IEquatable<time_t>, IComparable, IComparable<time_t>, IFormattable
    {
        #region Static Constructors
        
        public static time_t FromSeconds(double s) => new time_t(s);

        #endregion

        #region Numeric Conversions

        public static implicit operator time_t(double s) => new time_t(s);
        
        public static explicit operator double(time_t t) => t.Seconds;

        #endregion

        #region Core Data
        
        public readonly double Seconds;

        private time_t(double seconds)
        {
            Seconds = seconds;
        }

        #endregion

        #region Relational Operators
        
        public static bool operator ==(time_t a, time_t b) =>  a.Equals(b);
        public static bool operator !=(time_t a, time_t b) => !a.Equals(b);
        
        public static bool operator < (time_t a, time_t b) => a.Seconds <  b.Seconds;
        public static bool operator > (time_t a, time_t b) => a.Seconds >  b.Seconds;
        public static bool operator <=(time_t a, time_t b) => a.Seconds <= b.Seconds;
        public static bool operator >=(time_t a, time_t b) => a.Seconds >= b.Seconds;

        #endregion

        #region Arithmetic Operators

        public static time_t operator +(time_t a, time_t b) => new time_t(a.Seconds + b.Seconds);
        public static time_t operator -(time_t a, time_t b) => new time_t(a.Seconds - b.Seconds);
        
        public static time_t operator *(time_t a, time_t b) => new time_t(a.Seconds * b.Seconds);
        public static time_t operator *(time_t a, double factor) => new time_t(a.Seconds * factor);
        public static time_t operator *(time_t a, int factor) => new time_t(a.Seconds * factor);
        
        public static time_t operator *(double factor, time_t a) => a * factor;
        public static time_t operator *(int factor, time_t a) => a * factor;
        
        public static time_t operator /(time_t a, time_t b) => new time_t(a.Seconds / b.Seconds);
        public static time_t operator /(time_t a, double factor) => new time_t(a.Seconds / factor);
        public static time_t operator /(time_t a, int factor) => new time_t(a.Seconds / factor);

        public static time_t operator %(time_t a, time_t b) => new time_t(a.Seconds % b.Seconds);

        #endregion

        #region IComparable

        public int CompareTo(time_t other) => Seconds.CompareTo(other.Seconds);
        public int CompareTo(object obj)
        {
            if (obj is time_t other)
                return CompareTo(other);
            throw new ArgumentException($"Attempt to compare { obj?.GetType()?.FullName ?? "null" } to { typeof(time_t).FullName }");
        }

        #endregion

        #region IEquatable

        public bool Equals(time_t other) => Seconds == other.Seconds;
        public override bool Equals(object obj)
        {
            if (obj is time_t other)
                return Equals(other);
            return false;
        }

        #endregion
        
        public override int GetHashCode() => Seconds.GetHashCode();

        #region IFormattable

        public override string ToString() => ToString("G", null);
        public string ToString(string format) => ToString(format, null);
        public string ToString(IFormatProvider provider) => ToString("G", provider);

        /// <summary>
        /// Format options:
        /// 
        /// - "S" for seconds, displaying the time as a decimal
        ///   value followed by the abbreviation "s".
        /// 
        /// - "M" for milliseconds, displaying the time as a decimal
        ///   value followed by the abbreviation "ms".
        /// 
        /// - "U" for microseconds, displaying the time as an integral
        ///   value followed by the abbreviation "μs".
        /// 
        /// - "SF" for seconds, displaying the time as a decimal
        ///   value followed by the word "Seconds".
        ///   
        /// - "MF" for milliseconds, displaying the time as a decimal
        ///   value followed by the word "Millieconds".
        ///   
        /// - "UF" for microseconds, displaying the time as an integral
        ///   value followed by the word "Microseconds".
        ///   
        /// - "G" or any invalid string defaults to "SF".
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
                case "SF": return $"{ Seconds.ToString(provider) } Seconds";
                case "MF": return $"{ ((long)(Seconds * 1_000)).ToString(provider) } Milliseconds";
                case "UF": return $"{ ((long)(Seconds * 1_000_000)).ToString(provider) } Microseconds";
                    
                case "S": return $"{ Seconds.ToString(provider) } s";
                case "M": return $"{ ((long)(Seconds * 1_000)).ToString(provider) } ms";
                case "U": return $"{ ((long)(Seconds * 1_000_000)).ToString(provider) } μs";
            }
        }

        #endregion
    }
}
