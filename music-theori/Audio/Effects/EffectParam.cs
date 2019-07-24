using System;
using System.Collections.Generic;

namespace theori.Audio.Effects
{
    public class EffectParamI : EffectParam<int>
    {
        public static implicit operator EffectParamI(int value) => new EffectParamI(value);

        private readonly CubicBezier m_curve;

        public readonly Ease Ease;

        public EffectParamI(int value)
            : base(value)
        {
        }

        public EffectParamI(int a, int b, Ease ease)
            : base(a, b)
        {
            m_curve = new CubicBezier(ease);
            Ease = ease;
        }

        protected override int Interp(int min, int max, float alpha) => (int)(m_curve.Sample(alpha) * (max - min)) + min;
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

        public readonly int MinValueReal, MaxValueReal;

        public EffectParamX(int value)
            : base(1.0f / pieces[ValueToIndex(value)])
        {
            MinValueReal = MaxValueReal = value;
        }

        public EffectParamX(int valueMin, int valueMax)
            : base(ValueToIndex(valueMin), ValueToIndex(valueMax), Ease.Linear)
        {
            MinValueReal = valueMin;
            MaxValueReal = valueMax;
        }

        protected override float Interp(float a, float b, float t) => 1.0f / pieces[MathL.RoundToInt(a + (b - a) * t)];
    }

    public class EffectParamF : EffectParam<float>
    {
        public static implicit operator EffectParamF(float value) => new EffectParamF(value);

        private readonly CubicBezier m_curve;

        public readonly Ease Ease;

        public EffectParamF(float value)
            : base(value)
        {
        }

        public EffectParamF(float a, float b, Ease ease)
            : base(a, b)
        {
            m_curve = new CubicBezier(ease);
            Ease = ease;
        }

        protected override float Interp(float min, float max, float alpha) => m_curve.Sample(alpha) * (max - min) + min;
    }

    public class EffectParamS : EffectParam<string>
    {
        public static implicit operator EffectParamS(string value) => new EffectParamS(value);

        public EffectParamS(string value)
            : base(value)
        {
        }

        protected override string Interp(string min, string max, float alpha) => MinValue;
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

        public bool IsRange { get; private set; }

        public T MinValue => m_values[0];
        public T MaxValue => m_values[m_values.Length - 1];

        protected EffectParam(T value)
        {
            m_values = new T[] { value };
            IsRange = false;
        }

        protected EffectParam(T a, T b)
        {
            m_values = new T[] { a, b };
            IsRange = true;
        }

        protected abstract T Interp(T min, T max, float alpha);

        public T Sample(float alpha = 0)
        {
            alpha = MathL.Clamp(alpha, 0, 1);
            return IsRange ? Interp(m_values[0], m_values[1], alpha) : m_values[0];
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
