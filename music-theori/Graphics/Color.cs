using System;
using System.Numerics;

namespace theori.Graphics
{
    public static class Color
    {
        public static Vector3 HexToVector3(int rgb)
        {
            //       RR GG BB  (not necessary, but explains that there should ONLY be these bits)
            rgb &= 0xFF_FF_FF;

            float c(int i) => ((rgb >> (i * sizeof(byte))) & 0xFF) / 255.0f;
            return new Vector3(c(2), c(1), c(0));
        }

        public static int Vector3ToHex(Vector3 rgb)
        {
            int c(float v, int i) => (MathL.RoundToInt(v * 255) & 0xFF) << (i * sizeof(byte));
            return c(rgb.X, 2) | c(rgb.Y, 1) | c(rgb.Z, 0);
        }

        public static Vector3 HSVtoRGB(Vector3 hsv)
        {
            Vector4 K = new Vector4(1, 2.0f / 3, 1.0f / 3, 3);
            Vector3 p = MathL.Abs(MathL.Fract(new Vector3(hsv.X) + K.XYZ()) * 6 - new Vector3(K.W));
            return hsv.Z * MathL.Lerp(new Vector3(K.X), MathL.Clamp01(p - new Vector3(K.X)), hsv.Y);
        }

        public static Vector3 RGBtoHSV(Vector3 rgb)
        {
            Vector4 K = new Vector4(0, -1.0f / 3, 2.0f / 3, -1);
            //Vector4 p = MathL.Lerp(new Vector4(rgb.Z, rgb.Y, K.W, K.Z), new Vector4(rgb.Y, rgb.Z, K.X, K.Y), rgb.Y < rgb.Z ? 0 : 1);
            Vector4 p = rgb.Y < rgb.Z ? new Vector4(rgb.Z, rgb.Y, K.W, K.Z) : new Vector4(rgb.Y, rgb.Z, K.X, K.Y);
            //vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
            Vector4 q = p.X < rgb.X ? new Vector4(p.X, p.Y, p.W, rgb.X) : new Vector4(rgb.X, p.Y, p.Z, p.X);

            float d = q.X - MathL.Min(q.W, q.Y);
            float e = 1.0e-10f;
            return new Vector3(MathL.Abs(q.Z + (q.W - q.Y) / (6.0f * d + e)), d / (q.X + e), q.X);
        }
    }
}
