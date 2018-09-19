using System;

namespace OpenRM.Audio.Effects
{
    public delegate T EffectInterpolator<T>(T a, T b, float alpha);

    public class EffectParamI : EffectParam<int>
    {
        public static implicit operator EffectParamI(int value) => new EffectParamI(value);

        private static readonly EffectInterpolator<int> Lerp =
            (a, b, t) => (int)(a + (b - a)  * t);

        public EffectParamI(int value)
            : base(value)
        {
        }

        public EffectParamI(int a, int b, EffectInterpolator<int> interp = null)
            : base(a, b, interp ?? Lerp)
        {
        }

        public EffectParamI(int a, int b, CubicBezier curve)
            : base(a, b, (x, y, t) => (int)(curve.Sample(t) * (y - x)) + x)
        {
        }
    }

    public class EffectParamF : EffectParam<float>
    {
        public static implicit operator EffectParamF(float value) => new EffectParamF(value);

        private static readonly EffectInterpolator<float> Lerp =
            (a, b, t) => a + (b - a)  * t;

        public EffectParamF(float value)
            : base(value)
        {
        }

        public EffectParamF(float a, float b, EffectInterpolator<float> interp = null)
            : base(a, b, interp ?? Lerp)
        {
        }

        public EffectParamF(float a, float b, CubicBezier curve)
            : base(a, b, (x, y, t) => curve.Sample(t) * (y - x) + x)
        {
        }
    }

    public class EffectParam<T>
    {
        public static implicit operator EffectParam<T>(T value) => new EffectParam<T>(value);

        private readonly T[] values;
        private EffectInterpolator<T> interpFunction;

        private bool IsRange { get; }

        public EffectParam(T value)
        {
            values = new T[] { value };
            IsRange = false;
        }

        public EffectParam(T a, T b, EffectInterpolator<T> interp)
        {
            values = new T[] { a, b };
            interpFunction = interp;
            IsRange = true;
        }

        public T Sample(float alpha = 0)
        {
            alpha = MathL.Clamp(alpha, 0, 1);
            return IsRange ? interpFunction(values[0], values[1], alpha) : values[0];
        }
    }
}
