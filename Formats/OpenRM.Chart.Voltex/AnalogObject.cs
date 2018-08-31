﻿using System;

namespace OpenRM.Voltex
{
    public enum CurveShape
    {
        Linear,
        Cosine,
        ThreePoint,
    }

    public static class CurveShapeExt
    {
        public static float Sample(this CurveShape shape, float alpha, float a = 0.5f, float b = 0.5f)
        {
            switch (shape)
            {
                default:
                case CurveShape.Linear: return alpha;

                case CurveShape.Cosine:
                {
                    // TODO(local): "strengthen" the curve based on `a`
                    float angle = alpha * MathL.Pi_f;
                    return (1 - (float)Math.Cos(angle)) * 0.5f;
                }

                case CurveShape.ThreePoint:
                {
                    float t = (a - MathL.Sqrt(a * a + alpha - 2 * a * alpha)) / (-1 + 2 * a);
                    return 2 * (1 - t) * t * b + t * t;
                }
            }
        }
    }

    public sealed class AnalogObject : Object
    {
        private float m_initialValue, m_finalValue;
        private CurveShape m_shape = CurveShape.Linear;
        private float m_a, m_b;
        private bool m_extended;

        public AnalogObject Head => FirstConnectedOf<AnalogObject>();
        public AnalogObject Tail => LastConnectedOf<AnalogObject>();

        public bool IsSlam => IsInstant;

        public float InitialValue
        {
            get => m_initialValue;
            set => SetPropertyField(nameof(InitialValue), ref m_initialValue, value);
        }

        public float FinalValue
        {
            get => m_finalValue;
            set => SetPropertyField(nameof(FinalValue), ref m_finalValue, value);
        }

        public CurveShape Shape
        {
            get => m_shape;
            set => SetPropertyField(nameof(Shape), ref m_shape, value);
        }

        public float CurveA
        {
            get => m_a;
            set => SetPropertyField(nameof(CurveA), ref m_a, MathL.Clamp(value, 0, 1));
        }

        public float CurveB
        {
            get => m_b;
            set => SetPropertyField(nameof(CurveB), ref m_b, MathL.Clamp(value, 0, 1));
        }

        public bool RangeExtended
        {
            get => m_extended;
            set => SetPropertyField(nameof(RangeExtended), ref m_extended, value);
        }

        public float SampleValue(time_t position)
        {
            if (position <= AbsolutePosition) return InitialValue;
            if (position >= AbsoluteEndPosition) return FinalValue;

            return MathL.Lerp(InitialValue, FinalValue, (float)((position - AbsolutePosition).Seconds / AbsoluteDuration.Seconds));
        }
    }
}
