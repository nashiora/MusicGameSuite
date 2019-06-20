using System;

namespace OpenRM.Audio.Effects
{
    public delegate T EffectInterpolator<T>(T a, T b, float alpha);

    public class EffectParamI : EffectParam<int>
    {
        public static implicit operator EffectParamI(int value) => new EffectParamI(value);

        private static readonly EffectInterpolator<int> Lerp =
            (a, b, t) => (int)(a + (b - a) * t);

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

    public class EffectParamX : EffectParamF
    {
        private static readonly int[] pieces =
        {
            1, 2, 4, 6, 8, 12, 16, 24, 32, 48, 64
        };

        private static int ValueToIndex(int value)
        {
            for (int i = pieces.Length - 1; i >= 0; i--)
            {
                if (pieces[i] <= value)
                    return i;
            }
            return 0;
        }

        private static float Lerp(float a, float b, float t) => 1.0f / pieces[MathL.RoundToInt(a + (b - a) * t)];

        public EffectParamX(int value)
            : base(1.0f / pieces[ValueToIndex(value)])
        {
        }

        public EffectParamX(int valueMin, int valueMax)
            : base(ValueToIndex(valueMin), ValueToIndex(valueMax), Lerp)
        {
        }
    }

    public class EffectParamF : EffectParam<float>
    {
        public static implicit operator EffectParamF(float value) => new EffectParamF(value);

        private static readonly EffectInterpolator<float> Lerp =
            (a, b, t) => a + (b - a) * t;

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

    public class EffectParamS : EffectParam<string>
    {
        public static implicit operator EffectParamS(string value) => new EffectParamS(value);

        public EffectParamS(string value)
            : base(value)
        {
        }
    }

    public abstract class EffectParam
    {
    }

    public class EffectParam<T> : EffectParam
    {
        public static implicit operator EffectParam<T>(T value) => new EffectParam<T>(value);

        private readonly T[] m_values;
        private readonly EffectInterpolator<T> m_interpFunction;

        private readonly bool m_isRange;

        public EffectParam(T value)
        {
            m_values = new T[] { value };
            m_isRange = false;
        }

        public EffectParam(T a, T b, EffectInterpolator<T> interp)
        {
            m_values = new T[] { a, b };
            m_interpFunction = interp;
            m_isRange = true;
        }

        public T Sample(float alpha = 0)
        {
            alpha = MathL.Clamp(alpha, 0, 1);
            return m_isRange ? m_interpFunction(m_values[0], m_values[1], alpha) : m_values[0];
        }
    }
}
