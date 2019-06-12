using System.Numerics;

namespace System
{
    public static class MathL
    {
        private static Random random = new Random();
        
        /// <summary>
        /// Represents the ratio of the circumference of a circle to its diameter, specified
        //     by the constant, π.
        /// </summary>
        public const float Pi = 3.14159265358979323846f;
        public const float TwoPi = 2 * Pi;
        /// <summary>
        /// Represents the natural logarithmic base, specified by the constant, e.
        /// </summary>
        public const float E = 2.7182818284590452354f;
        
        public static Vector2 Abs(Vector2 value) => Vector2.Abs(value);
        public static Vector3 Abs(Vector3 value) => Vector3.Abs(value);
        public static Vector4 Abs(Vector4 value) => Vector4.Abs(value);
        public static decimal Abs(decimal value) => Math.Abs(value);
        public static double Abs(double value) => Math.Abs(value);
        public static float Abs(float value) => Math.Abs(value);
        public static long Abs(long value) => Math.Abs(value);
        public static int Abs(int value) => Math.Abs(value);
        public static short Abs(short value) => Math.Abs(value);
        public static sbyte Abs(sbyte value) => Math.Abs(value);
        
        public static ulong Absu(long value) => (ulong)Math.Abs(value);
        public static uint Absu(int value) => (uint)Math.Abs(value);
        public static ushort Absu(short value) => (ushort)Math.Abs(value);
        public static byte Absu(sbyte value) => (byte)Math.Abs(value);
        
        public static double Acos(double value) => Math.Acos(value);
        public static float Acos(float value) => (float)Math.Acos(value);

        public static double Asin(double value) => Math.Asin(value);
        public static float Asin(float value) => (float)Math.Asin(value);

        public static double Atan(double value) => Math.Atan(value);
        public static float Atan(float value) => (float)Math.Atan(value);

        public static double Atan(double y, double x) => Math.Atan2(y, x);
        public static float Atan(float y, float x) => (float)Math.Atan2(y, x);

        public static long BigMul(int a, int b) => Math.BigMul(a, b);
        
        public static decimal Ceil(decimal value) => Math.Ceiling(value);
        public static double Ceil(double value) => Math.Ceiling(value);
        public static float Ceil(float value) => (float)Math.Ceiling(value);
        
        public static long CeilToLong(decimal value) => (long)Math.Ceiling(value);
        public static long CeilToLong(double value) => (long)Math.Ceiling(value);
        public static long CeilToLong(float value) => (long)Math.Ceiling(value);

        public static int CeilToInt(double value) => (int)Math.Ceiling(value);
        public static int CeilToInt(float value) => (int)Math.Ceiling(value);
        
        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) => Vector2.Clamp(value, min, max);
        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max) => Vector3.Clamp(value, min, max);
        public static Vector4 Clamp(Vector4 value, Vector4 min, Vector4 max) => Vector4.Clamp(value, min, max);
        public static decimal Clamp(decimal value, decimal min, decimal max) => Max(min, Min(max, value));
        public static double Clamp(double value, double min, double max) => Max(min, Min(max, value));
        public static float Clamp(float value, float min, float max) => Max(min, Min(max, value));
        public static ulong Clamp(ulong value, ulong min, ulong max) => Max(min, Min(max, value));
        public static uint Clamp(uint value, uint min, uint max) => Max(min, Min(max, value));
        public static ushort Clamp(ushort value, ushort min, ushort max) => Max(min, Min(max, value));
        public static byte Clamp(byte value, byte min, byte max) => Max(min, Min(max, value));
        public static long Clamp(long value, long min, long max) => Max(min, Min(max, value));
        public static int Clamp(int value, int min, int max) => Max(min, Min(max, value));
        public static short Clamp(short value, short min, short max) => Max(min, Min(max, value));
        public static sbyte Clamp(sbyte value, sbyte min, sbyte max) => Max(min, Min(max, value));

        public static Vector3 Clamp01(Vector3 value) => Vector3.Clamp(value, Vector3.Zero, Vector3.One);
        public static decimal Clamp01(decimal value) => Max(0, Min(1, value));
        public static double Clamp01(double value) => Max(0, Min(1, value));
        public static float Clamp01(float value) => Max(0, Min(1, value));
        
        public static Vector2 ClampToLength(Vector2 value, float min, float max) => MaxLength(MinLength(value, max), min);
        public static Vector3 ClampToLength(Vector3 value, float min, float max) => MaxLength(MinLength(value, max), min);
        public static Vector4 ClampToLength(Vector4 value, float min, float max) => MaxLength(MinLength(value, max), min);

        public static double Cos(double radians) => Math.Cos(radians);
        public static float Cos(float radians) => (float)Math.Cos(radians);

        public static double Cosd(double degrees) => Math.Cos(ToRadians(degrees));
        public static float Cosd(float degrees) => (float)Math.Cos(ToRadians(degrees));

        public static double Cosh(double radians) => Math.Cosh(radians);
        public static float Cosh(float radians) => (float)Math.Cosh(radians);

        public static double Coshd(double degrees) => Math.Cosh(ToRadians(degrees));
        public static float Coshd(float degrees) => (float)Math.Cosh(ToRadians(degrees));

        public static int DivRem(int x, int y, out int remainder) => Math.DivRem(x, y, out remainder);
        public static long DivRem(long x, long y, out long remainder) => Math.DivRem(x, y, out remainder);

        public static double Exp(double value) => Math.Exp(value);
        public static float Exp(float value) => (float)Math.Exp(value);

        public static Vector4 Floor(Vector4 value) => new Vector4(Floor(value.X), Floor(value.Y), Floor(value.Z), Floor(value.W));
        public static Vector3 Floor(Vector3 value) => new Vector3(Floor(value.X), Floor(value.Y), Floor(value.Z));
        public static Vector2 Floor(Vector2 value) => new Vector2(Floor(value.X), Floor(value.Y));
        public static decimal Floor(decimal value) => Math.Floor(value);
        public static double Floor(double value) => Math.Floor(value);
        public static float Floor(float value) => (float)Math.Floor(value);
        
        public static long FloorToLong(decimal value) => (long)Math.Floor(value);
        public static long FloorToLong(double value) => (long)Math.Floor(value);
        public static long FloorToLong(float value) => (long)Math.Floor(value);

        public static int FloorToInt(double value) => (int)Math.Floor(value);
        public static int FloorToInt(float value) => (int)Math.Floor(value);

        public static Vector4 Fract(Vector4 value) => value - Floor(value);
        public static Vector3 Fract(Vector3 value) => value - Floor(value);
        public static Vector2 Fract(Vector2 value) => value - Floor(value);
        public static decimal Fract(decimal value) => value - Floor(value);
        public static double Fract(double value) => value - Floor(value);
        public static float Fract(float value) => value - Floor(value);

        public static double IEEERemainder(double x, double y) => Math.IEEERemainder(x, y);

        public static float InverseSqrt(float value) => 1.0f / (float)Math.Sqrt(value);
        
        public static double Lerp(double from, double to, double amount) => from + (to - from) * amount;
        public static float Lerp(float from, float to, float amount) => from + (to - from) * amount;
        public static Vector2 Lerp(Vector2 start, Vector2 end, float amount) =>       Vector2.Lerp(start, end, amount);
        public static Vector3 Lerp(Vector3 start, Vector3 end, float amount) =>       Vector3.Lerp(start, end, amount);
        public static Vector4 Lerp(Vector4 start, Vector4 end, float amount) =>       Vector4.Lerp(start, end, amount);
        public static Vector2 Lerp(Vector2 start, Vector2 end, Vector2 amount) => new Vector2(Lerp(start.X, end.X, amount.X), Lerp(start.Y, end.Y, amount.Y));
        public static Vector3 Lerp(Vector3 start, Vector3 end, Vector3 amount) => new Vector3(Lerp(start.X, end.X, amount.X), Lerp(start.Y, end.Y, amount.Y), Lerp(start.Z, end.Z, amount.Z));
        public static Vector4 Lerp(Vector4 start, Vector4 end, Vector4 amount) => new Vector4(Lerp(start.X, end.X, amount.X), Lerp(start.Y, end.Y, amount.Y), Lerp(start.Z, end.Z, amount.Z), Lerp(start.W, end.W, amount.W));
        
        public static Quaternion Lerp(Quaternion start, Quaternion end, float alpha)
        {
            float t = alpha;
            float t1 = 1.0f - t;

            Quaternion r;

            float dot = start.X * end.X + start.Y * end.Y +
                        start.Z * end.Z + start.W * end.W;

            if (dot >= 0.0f)
            {
                r.X = t1 * start.X + t * end.X;
                r.Y = t1 * start.Y + t * end.Y;
                r.Z = t1 * start.Z + t * end.Z;
                r.W = t1 * start.W + t * end.W;
            }
            else
            {
                r.X = t1 * start.X - t * end.X;
                r.Y = t1 * start.Y - t * end.Y;
                r.Z = t1 * start.Z - t * end.Z;
                r.W = t1 * start.W - t * end.W;
            }

            // Normalize it.
            float ls = r.X * r.X + r.Y * r.Y + r.Z * r.Z + r.W * r.W;
            float invNorm = 1.0f / Sqrt(ls);

            r.X *= invNorm;
            r.Y *= invNorm;
            r.Z *= invNorm;
            r.W *= invNorm;

            return r;
        }
        
        public static double LerpClamped(double from, double to, double amount) => Clamp(from + (to - from) * amount, from, to);
        public static float LerpClamped(float from, float to, float amount) => Clamp(from + (to - from) * amount, from, to);

        public static double Log(double value, double newBase) => Math.Log(value, newBase);
        public static float Log(float value, float newBase) => (float)Math.Log(value, newBase);

        public static double Log(double value) => Math.Log(value);
        public static float Log(float value) => (float)Math.Log(value);
        
        public static double Log10(double value) => Math.Log10(value);
        public static float Log10(float value) => (float)Math.Log10(value);

        public static decimal Max(decimal val1, decimal val2) => Math.Max(val1, val2);
        public static double Max(double val1, double val2) => Math.Max(val1, val2);
        public static float Max(float val1, float val2) => Math.Max(val1, val2);
        public static ulong Max(ulong val1, ulong val2) => Math.Max(val1, val2);
        public static uint Max(uint val1, uint val2) => Math.Max(val1, val2);
        public static ushort Max(ushort val1, ushort val2) => Math.Max(val1, val2);
        public static byte Max(byte val1, byte val2) => Math.Max(val1, val2);
        public static long Max(long val1, long val2) => Math.Max(val1, val2);
        public static int Max(int val1, int val2) => Math.Max(val1, val2);
        public static short Max(short val1, short val2) => Math.Max(val1, val2);
        public static sbyte Max(sbyte val1, sbyte val2) => Math.Max(val1, val2);
        
        public static Vector2 Max(Vector2 val1, Vector2 val2) => Vector2.Min(val1, val2);
        public static Vector3 Max(Vector3 val1, Vector3 val2) => Vector3.Min(val1, val2);
        public static Vector4 Max(Vector4 val1, Vector4 val2) => Vector4.Min(val1, val2);

        public static Vector2 MaxLength(Vector2 value, float maxLength) => value.Length() > maxLength ? Vector2.Normalize(value) * maxLength : value;
        public static Vector3 MaxLength(Vector3 value, float maxLength) => value.Length() > maxLength ? Vector3.Normalize(value) * maxLength : value;
        public static Vector4 MaxLength(Vector4 value, float maxLength) => value.Length() > maxLength ? Vector4.Normalize(value) * maxLength : value;

        public static decimal Min(decimal val1, decimal val2) => Math.Min(val1, val2);
        public static double Min(double val1, double val2) => Math.Min(val1, val2);
        public static float Min(float val1, float val2) => Math.Min(val1, val2);
        public static ulong Min(ulong val1, ulong val2) => Math.Min(val1, val2);
        public static uint Min(uint val1, uint val2) => Math.Min(val1, val2);
        public static ushort Min(ushort val1, ushort val2) => Math.Min(val1, val2);
        public static byte Min(byte val1, byte val2) => Math.Min(val1, val2);
        public static long Min(long val1, long val2) => Math.Min(val1, val2);
        public static int Min(int val1, int val2) => Math.Min(val1, val2);
        public static short Min(short val1, short val2) => Math.Min(val1, val2);
        public static sbyte Min(sbyte val1, sbyte val2) => Math.Min(val1, val2);
        
        public static Vector2 Min(Vector2 val1, Vector2 val2) => Vector2.Min(val1, val2);
        public static Vector3 Min(Vector3 val1, Vector3 val2) => Vector3.Min(val1, val2);
        public static Vector4 Min(Vector4 val1, Vector4 val2) => Vector4.Min(val1, val2);

        public static Vector2 MinLength(Vector2 value, float minLength) => value.Length() < minLength ? Vector2.Normalize(value) * minLength : value;
        public static Vector3 MinLength(Vector3 value, float minLength) => value.Length() < minLength ? Vector3.Normalize(value) * minLength : value;
        public static Vector4 MinLength(Vector4 value, float minLength) => value.Length() < minLength ? Vector4.Normalize(value) * minLength : value;

        public static double Pow(double x, double n) => Math.Pow(x, n);
        public static float Pow(float x, float n) => (float)Math.Pow(x, n);
        
        public static double Random(double min, double max) => random.NextDouble() * (max - min) + min;
        public static float Random(float min, float max) => (float)Random((double)min, (double)max);

        public static decimal Round(decimal value, int decimals, MidpointRounding mode) => Math.Round(value, decimals, mode);
        public static decimal Round(decimal value, MidpointRounding mode) => Math.Round(value, mode);
        public static decimal Round(decimal value, int decimals) => Math.Round(value, decimals);
        public static decimal Round(decimal value) => Math.Round(value);

        public static double Round(double value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode);
        public static double Round(double value, MidpointRounding mode) => Math.Round(value, mode);
        public static double Round(double value, int digits) => Math.Round(value, digits);
        public static double Round(double value) => Math.Round(value);
        
        public static float Round(float value, int digits, MidpointRounding mode) => (float)Math.Round(value, digits, mode);
        public static float Round(float value, MidpointRounding mode) => (float)Math.Round(value, mode);
        public static float Round(float value, int digits) => (float)Math.Round(value, digits);
        public static float Round(float value) => (float)Math.Round(value);

        public static long RoundToLong(decimal value, MidpointRounding mode) => (long)Math.Round(value, mode);
        public static long RoundToLong(decimal value) => (long)Math.Round(value);

        public static long RoundToLong(double value, MidpointRounding mode) => (long)Math.Round(value, mode);
        public static long RoundToLong(double value) => (long)Math.Round(value);

        public static long RoundToLong(float value, MidpointRounding mode) => (long)Math.Round(value, mode);
        public static long RoundToLong(float value) => (long)Math.Round(value);

        public static int RoundToInt(double value, MidpointRounding mode) => (int)Math.Round(value, mode);
        public static int RoundToInt(double value) => (int)Math.Round(value);

        public static int RoundToInt(float value, MidpointRounding mode) => (int)Math.Round(value, mode);
        public static int RoundToInt(float value) => (int)Math.Round(value);

        public static int Sign(decimal value) => Math.Sign(value);
        public static int Sign(double value) => Math.Sign(value);
        public static int Sign(float value) => Math.Sign(value);
        public static int Sign(long value) => Math.Sign(value);
        public static int Sign(int value) => Math.Sign(value);
        public static int Sign(short value) => Math.Sign(value);
        public static int Sign(sbyte value) => Math.Sign(value);

        public static double Sin(double radians) => Math.Sin(radians);
        public static float Sin(float radians) => (float)Math.Sin(radians);

        public static double Sind(double degrees) => Math.Sin(ToRadians(degrees));
        public static float Sind(float degrees) => (float)Math.Sin(ToRadians(degrees));
        
        public static double Sinh(double radians) => Math.Sinh(radians);
        public static float Sinh(float radians) => (float)Math.Sinh(radians);
        
        public static double Sinhd(double degrees) => Math.Sinh(ToRadians(degrees));
        public static float Sinhd(float degrees) => (float)Math.Sinh(ToRadians(degrees));

        public static Quaternion Slerp(Quaternion start, Quaternion end, float alpha)
        {
            const float epsilon = 1e-6f;

            float t = alpha;

            float cosOmega = start.X * end.X + start.Y * end.Y +
                             start.Z * end.Z + start.W * end.W;

            bool flip = false;

            if (cosOmega < 0.0f)
            {
                flip = true;
                cosOmega = -cosOmega;
            }

            float s1, s2;

            if (cosOmega > (1.0f - epsilon))
            {
                // Too close, do straight linear interpolation.
                s1 = 1.0f - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                float omega = Acos(cosOmega);
                float invSinOmega = 1 / Sin(omega);

                s1 = Sin((1.0f - t) * omega) * invSinOmega;
                s2 = (flip)
                    ? -Sin(t * omega) * invSinOmega
                    :  Sin(t * omega) * invSinOmega;
            }

            Quaternion r;

            r.X = s1 * start.X + s2 * end.X;
            r.Y = s1 * start.Y + s2 * end.Y;
            r.Z = s1 * start.Z + s2 * end.Z;
            r.W = s1 * start.W + s2 * end.W;

            return r;
        }

        public static Vector2 Square(Vector2 value) => value * value;
        public static Vector3 Square(Vector3 value) => value * value;
        public static Vector4 Square(Vector4 value) => value * value;
        public static double Square(double value) => value * value;
        public static float Square(float value) => value * value;

        public static Vector2 Sqrt(Vector2 value) => Vector2.SquareRoot(value);
        public static Vector3 Sqrt(Vector3 value) => Vector3.SquareRoot(value);
        public static Vector4 Sqrt(Vector4 value) => Vector4.SquareRoot(value);
        public static double Sqrt(double value) => Math.Sqrt(value);
        public static float Sqrt(float value) => (float)Math.Sqrt(value);

        public static double Tan(double radians) => Math.Tan(radians);
        public static float Tan(float radians) => (float)Math.Tan(radians);

        public static double Tand(double degrees) => Math.Tan(ToRadians(degrees));
        public static float Tand(float degrees) => (float)Math.Tan(ToRadians(degrees));
        
        public static double Tanh(double radians) => Math.Tanh(radians);
        public static float Tanh(float radians) => (float)Math.Tanh(radians);
        
        public static double Tanhd(double degrees) => Math.Tanh(ToRadians(degrees));
        public static float Tanhd(float degrees) => (float)Math.Tanh(ToRadians(degrees));
        
        public static double ToDegrees(double radians) => radians * 180 / Pi;
        public static float ToDegrees(float radians) => radians * 180 / Pi;
        
        public static double ToRadians(double degrees) => degrees * Pi / 180;
        public static float ToRadians(float degrees) => degrees * Pi / 180;

        public static decimal Truncate(decimal value) => Math.Truncate(value);
        public static double Truncate(double value) => Math.Truncate(value);
        public static float Truncate(float value) => (float)Math.Truncate(value);
    }
}
