using System;
using System.Numerics;

namespace FxMania
{
    public static class Mathf
    {
        public const float Pi = 3.1415926535897932384626f;
        
        public static float ToDegrees(float rad) => rad * 180 / Pi;
        public static float ToRadians(float deg) => deg * Pi / 180;
        
        public static float Abs (float x) => Math.Abs(x);
        public static float Sign(float x) => Math.Sign(x);

        public static int   Min(int   a, int   b) => a < b ? a : b;
        public static float Min(float a, float b) => a < b ? a : b;
        
        public static int   Max(int   a, int   b) => a > b ? a : b;
        public static float Max(float a, float b) => a > b ? a : b;
        
        public static int   Clamp(int   value, int   min, int   max) => Max(min, Min(max, value));
        public static float Clamp(float value, float min, float max) => Max(min, Min(max, value));
        
        public static int   Lerp(int   start, int   end, float alpha) => (int)(start + (end - start) * alpha);
        public static float Lerp(float start, float end, float alpha) =>       start + (end - start) * alpha;
        public static Vector2 Lerp(Vector2 start, Vector2 end, float alpha) =>       start + (end - start) * alpha;
        public static Vector3 Lerp(Vector3 start, Vector3 end, float alpha) =>       start + (end - start) * alpha;
        public static Vector4 Lerp(Vector4 start, Vector4 end, float alpha) =>       start + (end - start) * alpha;
        public static Vector2 Lerp(Vector2 start, Vector2 end, Vector2 alpha) => new Vector2(Lerp(start.X, end.X, alpha.X), Lerp(start.Y, end.Y, alpha.Y));
        public static Vector3 Lerp(Vector3 start, Vector3 end, Vector3 alpha) => new Vector3(Lerp(start.X, end.X, alpha.X), Lerp(start.Y, end.Y, alpha.Y), Lerp(start.Z, end.Z, alpha.Z));
        public static Vector4 Lerp(Vector4 start, Vector4 end, Vector4 alpha) => new Vector4(Lerp(start.X, end.X, alpha.X), Lerp(start.Y, end.Y, alpha.Y), Lerp(start.Z, end.Z, alpha.Z), Lerp(start.W, end.W, alpha.W));
        
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

        public static float        Sqrt(float value) =>        (float)Math.Sqrt(value);
        public static float InverseSqrt(float value) => 1.0f / (float)Math.Sqrt(value);
        
        public static float Pow(float a, float b) => (float)Math.Pow(a, b);
        public static float Exp(float value)      => (float)Math.Exp(value);

        public static float Sin(float value) => (float)Math.Sin(value);
        public static float Cos(float value) => (float)Math.Cos(value);
        public static float Tan(float value) => (float)Math.Tan(value);
        
        public static float Asin(float value) => (float)Math.Asin(value);
        public static float Acos(float value) => (float)Math.Acos(value);
        public static float Atan(float value) => (float)Math.Atan(value);
        public static float Atan(float y, float x) => (float)Math.Atan2(y, x);
    }
}
