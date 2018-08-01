using System;
using System.Collections.Generic;

namespace OpenRM
{
    public class Object : ILinkable<Object>, IComparable<Object>, ICloneable
    {
        private tick_t m_position, m_duration;
        private time_t m_calcPosition = (time_t)long.MinValue,
                       m_calcDuration = (time_t)long.MinValue;

        internal int m_stream;

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
                    m_calcPosition = cp.AbsolutePosition + cp.BeatDuration * (m_position - cp.Position);
                }
                return m_calcPosition;
            }
        }

        public time_t AbsoluteEndPosition => AbsolutePosition + AbsoluteDuration;

        public time_t AbsoluteDuration
        {
            get
            {
                if (Chart == null)
                    throw new InvalidOperationException("Cannot calculate the absolute duration of an object without an assigned Chart.");

                if (m_calcDuration == (time_t)long.MinValue)
                {
                    tick_t endPos = m_position + m_duration;
                    ControlPoint cp = Chart.ControlPoints.MostRecent(endPos);
                    m_calcDuration = cp.AbsolutePosition + cp.BeatDuration * (endPos - cp.Position) - AbsolutePosition;
                }
                return m_calcDuration;
            }
        }

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
        
        public Object Previous => ((ILinkable<Object>)this).Previous;
        Object ILinkable<Object>.Previous { get; set; }

        public Object Next => ((ILinkable<Object>)this).Next;
        Object ILinkable<Object>.Next { get; set; }

        public Chart Chart { get; internal set; }

        public Object()
        {
        }

        public virtual Object Clone()
        {
            var result = new Object()
            {
                m_position = m_position,
                m_duration = m_duration,
                m_stream = m_stream,
            };
            return result;
        }

        object ICloneable.Clone() => Clone();

        public virtual int CompareTo(Object other)
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
            return m_duration.CompareTo(other.m_duration);
        }
        
        int IComparable<Object>.CompareTo(Object other) => CompareTo(other);

        internal void InvalidateTimingCalc()
        {
            m_calcPosition = (time_t)long.MinValue;
            m_calcDuration = (time_t)long.MinValue;
        }
    }

    public abstract class ObjectData
    {
        public delegate void PropertyChangedEventHandler(ObjectData sender, PropertyChangedEventArgs args);

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

        internal event PropertyChangedEventHandler PropertyChanged;

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

    public sealed class DictObjectData : ObjectData
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

    public class Object<TData> : Object
        where TData : ObjectData
    {
        private TData data;
        public TData Data
        {
            get => data;
            set
            {
                if (value == data)
                    return;

                if (data != null)
                    data.PropertyChanged -= OnDataPropertyChanged;

                data = value;
                if (data != null)
                    data.PropertyChanged += OnDataPropertyChanged;
            }
        }

        public event ObjectData.PropertyChangedEventHandler DataPropertyChanged;

        private void OnDataPropertyChanged(ObjectData sender, ObjectData.PropertyChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"{ args.PropertyName } changed for object");

            DataPropertyChanged?.Invoke(sender, args);
        }
    }
}
