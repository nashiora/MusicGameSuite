using System;
using System.Collections.Generic;

namespace theori.Charting
{
    public class ChartObject : ILinkable<ChartObject>, IComparable<ChartObject>, ICloneable
    {
        private static long creationIndexCounter = 0;

        private readonly long m_id = ++creationIndexCounter;

        private tick_t m_position, m_duration;
        private time_t m_calcPosition = (time_t)long.MinValue,
                       m_calcEndPosition = (time_t)long.MinValue;

        internal int m_stream;

        /// <summary>
        /// The position, in beats, of this object.
        /// </summary>
        [SerializeField]
        public tick_t Position
        {
            get => m_position;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Objects cannot have negative positions.", nameof(Position));

                m_position = value;
                InvalidateTimingCalc();
            }
        }
        
        [SerializeField]
        public tick_t Duration
        {
            get => m_duration;
            set { m_duration = value; InvalidateTimingCalc(); }
        }

        public tick_t EndPosition => Position + Duration;

        public bool IsInstant => m_duration == 0;

        public time_t AbsolutePosition
        {
            get
            {
                if (Chart == null)
                    throw new InvalidOperationException("Cannot calculate the absolute position of an object without an assigned Chart.");

                if (m_calcPosition == (time_t)long.MinValue)
                {
                    ControlPoint cp = Chart.ControlPoints.MostRecent(m_position);
                    m_calcPosition = cp.AbsolutePosition + cp.MeasureDuration * (m_position - cp.Position);
                }
                return m_calcPosition;
            }
        }

        public time_t AbsoluteEndPosition
        {
            get
            {
                if (Chart == null)
                    throw new InvalidOperationException("Cannot calculate the absolute duration of an object without an assigned Chart.");

                if (m_calcEndPosition == (time_t)long.MinValue)
                {
                    ControlPoint cp = Chart.ControlPoints.MostRecent(EndPosition);
                    m_calcEndPosition = cp.AbsolutePosition + cp.MeasureDuration * (EndPosition - cp.Position);
                }
                return m_calcEndPosition;
            }
        }

        public time_t AbsoluteDuration => AbsoluteEndPosition - AbsolutePosition;

        public int Stream
        {
            get => m_stream;
            set
            {
                var chart = Chart;
                if (chart != null)
                {
                    chart.ObjectStreams[m_stream].Remove(this);
                    // will re-assign Chart and m_stream
                    chart.ObjectStreams[value].Add(this);
                }
                else m_stream = value;
            }
        }
        
        public bool HasPrevious => Previous != null;
        public bool HasNext => Next != null;
        
        public ChartObject Previous => ((ILinkable<ChartObject>)this).Previous;
        ChartObject ILinkable<ChartObject>.Previous { get; set; }

        public ChartObject Next => ((ILinkable<ChartObject>)this).Next;
        ChartObject ILinkable<ChartObject>.Next { get; set; }

        public ChartObject PreviousConnected
        {
            get
            {
                var p = Previous;
                return p != null && p.EndPosition == Position ? p : null;
            }
        }

        public ChartObject NextConnected
        {
            get
            {
                var n = Next;
                return n != null && n.Position == EndPosition ? n : null;
            }
        }

        public T FirstConnectedOf<T>()
            where T : ChartObject
        {
            var current = this as T;
            while (current?.PreviousConnected is T prev)
                current = prev;
            return current;
        }

        public T LastConnectedOf<T>()
            where T : ChartObject
        {
            var current = this as T;
            while (current?.NextConnected is T next)
                current = next;
            return current;
        }

        public Chart Chart { get; internal set; }

        public ChartObject()
        {
        }

        public virtual ChartObject Clone()
        {
            var result = new ChartObject()
            {
                m_position = m_position,
                m_duration = m_duration,
                m_stream = m_stream,
            };
            return result;
        }

        object ICloneable.Clone() => Clone();

        public virtual int CompareTo(ChartObject other)
        {
            int r = m_position.CompareTo(other.m_position);
            if (r != 0)
                return r;

            if (m_duration == 0)
            {
                if (other.m_duration != 0)
                    return -1;
            }
            else if (other.m_duration == 0)
                return 1;

            // oh well, we tried
            return m_id.CompareTo(other.m_id);;
        }
        
        int IComparable<ChartObject>.CompareTo(ChartObject other) => CompareTo(other);

        internal void InvalidateTimingCalc()
        {
            m_calcPosition = (time_t)long.MinValue;
            m_calcEndPosition = (time_t)long.MinValue;
        }

        public delegate void PropertyChangedEventHandler(ChartObject sender, PropertyChangedEventArgs args);

        [Flags]
        public enum Invalidation
        {
            None = 0,
        }

        public sealed class PropertyChangedEventArgs
        {
            public string PropertyName { get; set; }
            public Invalidation Invalidation { get; set; }

            public PropertyChangedEventArgs(string propertyName)
            {
                PropertyName = propertyName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName, Invalidation invalidation = Invalidation.None)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName) { Invalidation = invalidation });
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        protected void SetPropertyField<T>(string propertyName, ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    public sealed class DictObject : ChartObject
    {
        private readonly Dictionary<string, Variant> values = new Dictionary<string, Variant>();

        public Variant this[string name]
        {
            get
            {
                if (values.TryGetValue(name, out var result))
                    return result;
                return Variant.Null;
            }

            set
            {
                if (values.TryGetValue(name, out var result))
                {
                    if (value == result)
                        return;
                }

                values[name] = value;
                OnPropertyChanged(name);
            }
        }
    }
}
