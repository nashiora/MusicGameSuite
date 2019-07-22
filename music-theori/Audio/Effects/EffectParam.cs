using System;
using System.Collections;
using System.Collections.Generic;

namespace theori.Audio.Effects
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

        public readonly int MinValueReal, MaxValueReal;

        public EffectParamX(int value)
            : base(1.0f / pieces[ValueToIndex(value)])
        {
            MinValueReal = MaxValueReal = value;
        }

        public EffectParamX(int valueMin, int valueMax)
            : base(ValueToIndex(valueMin), ValueToIndex(valueMax), Lerp)
        {
            MinValueReal = valueMin;
            MaxValueReal = valueMax;
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

    public interface IEffectParam : IEquatable<IEffectParam>
    {
        bool IsRange { get; }
    }

    public abstract class EffectParam<T> : IEffectParam, IEquatable<EffectParam<T>>
        where T : IEquatable<T>
    {
        public static bool operator ==(EffectParam<T> a, EffectParam<T> b) => a is null ? b is null : a.Equals(b);
        public static bool operator !=(EffectParam<T> a, EffectParam<T> b) => !(a == b);

        private readonly T[] m_values;
        private readonly EffectInterpolator<T> m_interpFunction;

        public bool IsRange { get; private set; }

        public T MinValue => m_values[0];
        public T MaxValue => m_values[m_values.Length - 1];

        protected EffectParam(T value)
        {
            m_values = new T[] { value };
            IsRange = false;
        }

        protected EffectParam(T a, T b, EffectInterpolator<T> interp)
        {
            m_values = new T[] { a, b };
            m_interpFunction = interp;
            IsRange = true;
        }

        public T Sample(float alpha = 0)
        {
            alpha = MathL.Clamp(alpha, 0, 1);
            return IsRange ? m_interpFunction(m_values[0], m_values[1], alpha) : m_values[0];
        }

        public bool Equals(EffectParam<T> other)
        {
            if (m_values.Length != other.m_values.Length) return false;
            for (int i = 0; i < m_values.Length; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(m_values[i], other.m_values[i]))
                    return false;
            }
            return true;
        }

        bool IEquatable<IEffectParam>.Equals(IEffectParam other)
        {
            if (other is EffectParam<T> t) return Equals(t);
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is EffectParam<T> t) return Equals(t);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.For(m_values);
        }
    }
}
