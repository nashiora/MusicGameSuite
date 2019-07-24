using System;
using System.Collections.Generic;

namespace theori.Charting
{
    public sealed class ControlPoint : ILinkable<ControlPoint>, IComparable<ControlPoint>, ICloneable
    {
        [Flags]
        public enum Property : byte
        {
            None = 0,

            BeatsPerMinute = 0x1,
            TimeSignature = 0x2,

            Timing = BeatsPerMinute | TimeSignature,

            All = Timing,
        }
        
        private tick_t m_position;
        private time_t m_calcPosition = (time_t)long.MinValue;

        private double m_bpm = 120;
        private time_t m_qnDuration = time_t.FromSeconds(60.0 / 120);
        private int m_beatCount = 4, m_beatKind = 4;

        [TheoriProperty("position")]
        public tick_t Position
        {
            get => m_position;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Control points cannot have negative positions.", nameof(Position));

                m_position = value;

                Chart?.InvalidateTimeCalc();
                Chart?.ControlPoints.Resort();
            }
        }

        /// <summary>
        /// Time Signature Numerator
        /// </summary>
        [TheoriProperty("numerator")]
        public int BeatCount
        {
            get => m_beatCount;
            set
            {
                if (value == m_beatCount)
                    return;
                m_beatCount = value;
                Chart?.InvalidateTimeCalc();
            }
        }

        /// <summary>
        /// Time Signature Denominator
        /// </summary>
        [TheoriProperty("denominator")]
        public int BeatKind
        {
            get => m_beatKind;
            set
            {
                if (value == m_beatKind)
                    return;
                m_beatKind = value;
                Chart?.InvalidateTimeCalc();
            }
        }

        [TheoriProperty("multiplier")]
        public double SpeedMultiplier { get; set; } = 1.0;

        public time_t AbsolutePosition
        {
            get
            {
                if (Chart == null)
                    throw new InvalidOperationException("Cannot calculate the absolute position of a control point without an assigned Chart.");

                if (m_calcPosition == (time_t)long.MinValue)
                {
                    var prev = Previous;
                    if (prev == null)
                        m_calcPosition = Chart.Offset;
                    else
                    {
                        m_calcPosition = prev.AbsolutePosition
                                       + prev.MeasureDuration * (m_position - prev.m_position);
                    }
                }

                return m_calcPosition;
            }
        }

        /// <summary>
        /// The number of quarter-note beats per minute.
        /// </summary>
        [TheoriProperty("bpm")]
        public double BeatsPerMinute
        {
            get => m_bpm;
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException(nameof(BeatsPerMinute), "BPM must be a positive value.");

                m_bpm = value;
                m_qnDuration = time_t.FromSeconds(60.0 / value);

                Chart?.InvalidateTimeCalc();
            }
        }
        
        public time_t QuarterNoteDuration => m_qnDuration;
        public time_t BeatDuration => QuarterNoteDuration * 4 / BeatKind;

        public time_t MeasureDuration => BeatDuration * BeatCount;
        
        public bool HasPrevious => Previous != null;
        public bool HasNext => Next != null;

        public ControlPoint Previous => ((ILinkable<ControlPoint>)this).Previous;
        ControlPoint ILinkable<ControlPoint>.Previous { get; set; }

        public ControlPoint Next => ((ILinkable<ControlPoint>)this).Next;
        ControlPoint ILinkable<ControlPoint>.Next { get; set; }

        [TheoriIgnore]
        public Chart Chart { get; internal set; }

        public ControlPoint()
        {
        }

        public ControlPoint Clone()
        {
            var result = new ControlPoint()
            {
                m_position = m_position,
                m_bpm = m_bpm,
                m_qnDuration = m_qnDuration,
                m_beatCount = m_beatCount,
                m_beatKind = m_beatKind,
            };
            return result;
        }

        object ICloneable.Clone() => Clone();

        int IComparable<ControlPoint>.CompareTo(ControlPoint other) => m_position.CompareTo(other.m_position);

        internal void InvalidateCalc()
        {
            m_calcPosition = (time_t)long.MinValue;
        }
    }
}
