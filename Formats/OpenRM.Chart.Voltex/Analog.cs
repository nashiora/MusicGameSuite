using System;

namespace OpenRM.Voltex
{
    public enum CurveShape
    {
        Linear,
        Cosine,
        ThreePoint,
    }

    public sealed class Analog : ObjectData
    {
        private float m_initialValue, m_finalValue;
        private CurveShape m_shape = CurveShape.Linear;
        private float m_a, m_b;
        private bool m_extended;

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

        public float PointA
        {
            get => m_a;
            set => SetPropertyField(nameof(PointA), ref m_a, MathL.Clamp(value, 0, 1));
        }

        public float PointB
        {
            get => m_b;
            set => SetPropertyField(nameof(PointB), ref m_b, MathL.Clamp(value, 0, 1));
        }

        public bool RangeExtended
        {
            get => m_extended;
            set => SetPropertyField(nameof(RangeExtended), ref m_extended, value);
        }
    }
}
