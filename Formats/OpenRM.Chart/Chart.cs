using System;
using System.Collections.Generic;

namespace OpenRM
{
    /// <summary>
    /// Contains all relevant data for a single chart.
    /// </summary>
    public sealed class Chart
    {
        public ChartMetadata Metadata;

        public readonly int StreamCount;

        /// <summary>
        /// Contains StreamCount ObjectStreams.
        /// Each object "stream" is an ordered linked list of objects.
        /// </summary>
        public readonly ObjectStream[] ObjectStreams;
        public readonly ControlPointList ControlPoints;

        public ObjectStream this[int stream] => ObjectStreams[stream];

        private time_t m_offset;
        public time_t Offset
        {
            get => m_offset;
            set
            {
                m_offset = value;
                InvalidateTimeCalc();
            }
        }

        public Chart(int streamCount)
        {
            StreamCount = streamCount;
            ObjectStreams = new ObjectStream[streamCount];
            for (int i = 0; i < streamCount; i++)
                ObjectStreams[i] = new ObjectStream(this, i);

            ControlPoints = new ControlPointList(this);
        }

        internal void InvalidateTimeCalc()
        {
            for (int i = 0; i < StreamCount; i++)
                ObjectStreams[i].InvalidateTimeCalc();
                
            ControlPoints.InvalidateTimeCalc();
        }

        /// <summary>
        /// Shortcut function which adds the given object to the stream it specifies.
        /// This is identical to `ObjectStreams[obj.Stream].Add(obj)`.
        /// </summary>
        public void AddObject(Object obj) => ObjectStreams[obj.Stream].Add(obj);

        public sealed class ObjectStream
        {
            private readonly Chart m_chart;
            private readonly int m_stream;
            private readonly OrderedLinkedList<Object> m_objects = new OrderedLinkedList<Object>();
            
            internal ObjectStream(Chart chart, int stream)
            {
                m_chart = chart;
                m_stream = stream;
            }

            internal void InvalidateTimeCalc()
            {
                foreach (var obj in m_objects) obj.InvalidateTimingCalc();
            }

            /// <summary>
            /// When the given object already belongs to this chart at
            ///  the same stream, this is a no-op.
            /// 
            /// If the given object is already assigned a non-null chart,
            ///  this throws an ArgumentException.
            ///  
            /// If the given object already belongs to this chart, but in
            ///  a different object stream, it is first removed from that stream.
            ///  
            /// This function adds the given object to this stream and assigns
            ///  the object's Chart and Stream fields accordingly.
            /// </summary>
            public void Add(Object obj)
            {
                System.Diagnostics.Debug.Assert(obj.Position >= 0);

                // already added in the correct place
                if (obj.Chart == m_chart && obj.Stream == m_stream)
                    return;

                if (obj.Chart != null)
                {
                    if (obj.Chart != m_chart)
                        throw new ArgumentException("Given object is parented to a different chart!.", nameof(obj));
                    m_chart.ObjectStreams[obj.m_stream].Remove(obj);
                }

                obj.Chart = m_chart;
                obj.m_stream = m_stream;

                m_objects.Add(obj);
            }

            /// <summary>
            /// Constructs a new Object and calls `Add(Object)` to add it to this stream.
            /// The newly created Object is returned.
            /// </summary>
            public Object Add(tick_t position, tick_t duration = default)
            {
                var obj = new Object()
                {
                    Position = position,
                    Duration = duration,
                };

                Add(obj);
                return obj;
            }

            /// <summary>
            /// Constructs a new Object&lt;<typeparamref name="TData"/>&gt; with
            ///  the given data and calls `Add(Object)` to add it to this stream.
            /// The newly created Object is returned.
            /// </summary>
            public Object<TData> Add<TData>(TData data, tick_t position, tick_t duration = default)
                where TData : ObjectData
            {
                var obj = new Object<TData>()
                {
                    Position = position,
                    Duration = duration,
                    Data = data,
                };

                Add(obj);
                return obj;
            }

            /// <summary>
            /// If the object is contained in this stream, it is removed and
            ///  its Chart property is set to null.
            /// </summary>
            /// <param name="obj"></param>
            public void Remove(Object obj)
            {
                if (obj.Chart == m_chart && obj.Stream == m_stream)
                {
                    bool rem = m_objects.Remove(obj);
                    System.Diagnostics.Debug.Assert(rem);
                    obj.Chart = null;
                }
            }

            /// <summary>
            /// Find the object, if any, at the current position.
            /// If `includeDuration` is true, the first object which contains
            ///  the given position is returned.
            /// </summary>
            public Object Find(tick_t position, bool includeDuration)
            {
                // TODO(local): make this a binary search?
                for (int i = 0, count = m_objects.Count; i < count; i++)
                {
                    var obj = m_objects[i];
                    if (includeDuration)
                    {
                        if (obj.Position <= position && obj.Duration >= position)
                            return obj;
                    }
                    else if (obj.Position == position)
                        return obj;
                }

                return null;
            }
        }

        public sealed class ControlPointList
        {
            private readonly Chart m_chart;
            private readonly OrderedLinkedList<ControlPoint> m_controlPoints = new OrderedLinkedList<ControlPoint>();

            public ControlPoint Root => m_controlPoints[0];

            internal ControlPointList(Chart chart)
            {
                m_chart = chart;
                Add(new ControlPoint());
            }

            internal void InvalidateTimeCalc()
            {
                foreach (var cp in m_controlPoints) cp.InvalidateCalc();
            }

            internal void Resort()
            {
                m_controlPoints.Sort();
                InvalidateTimeCalc();
            }

            public void Add(ControlPoint point)
            {
                System.Diagnostics.Debug.Assert(point.Position >= 0);

                // already added in the correct place
                if (point.Chart == m_chart)
                    return;

                if (point.Chart != null)
                    throw new ArgumentException("Given control point is parented to a different chart!.", nameof(point));

                foreach (var cp in m_controlPoints)
                {
                    if (cp.Position == point.Position)
                        throw new ArgumentException("Cannot add a control point whose position is identical to another. Use `GetControlPoint(tick_t)` instead.", nameof(point));
                }

                point.Chart = m_chart;
                m_controlPoints.Add(point);
            }

            public ControlPoint GetOrCreate(tick_t position, bool clonePrevious = true)
            {
                ControlPoint mostRecent = null;
                foreach (var cp in m_controlPoints)
                {
                    if (cp.Position == position)
                        return cp;
                    else if (cp.Position < position)
                        mostRecent = cp;
                }

                ControlPoint result;
                if (clonePrevious && mostRecent != null)
                    result = mostRecent.Clone();
                else result = new ControlPoint()
                {
                    Position = position,
                };

                Add(result);
                return result;
            }

            public void Remove(ControlPoint point)
            {
                if (point.Chart != m_chart)
                    return;

                if (point.Position == 0)
                    throw new InvalidOperationException("Cannot remove the root timing section of a chart!");
                m_controlPoints.Remove(point);
                point.Chart = null;
            }

            public ControlPoint MostRecent(tick_t position)
            {
                for (int i = 0, count = m_controlPoints.Count; i < count; i++)
                {
                    var cp = m_controlPoints[i];
                    if (cp.Position <= position)
                        return cp;
                }
                return null;
            }
        }
    }
}
