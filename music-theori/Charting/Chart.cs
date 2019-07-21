using System;
using System.Collections;
using System.Collections.Generic;
using theori.GameModes;

namespace theori.Charting
{
    /// <summary>
    /// Contains all relevant data for a single chart.
    /// </summary>
    public sealed class Chart
    {
        public ChartSetInfo SetInfo => Info.Set;
        public ChartInfo Info { get; set; } = new ChartInfo();

        public readonly int StreamCount;

        public readonly GameMode GameMode;

        /// <summary>
        /// Contains StreamCount ObjectStreams.
        /// Each object "stream" is an ordered linked list of objects.
        /// </summary>
        public readonly ObjectStream[] ObjectStreams;
        public readonly ControlPointList ControlPoints;

        public time_t LastObjectTime
        {
            get
            {
                time_t lastTime = double.MinValue;
                foreach (var stream in ObjectStreams)
                {
                    if (stream.Count == 0) continue;

                    time_t t = stream.LastObject.AbsolutePosition;
                    if (t > lastTime)
                        lastTime = t;
                }
                return lastTime;
            }
        }

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

        public tick_t TickEnd
        {
            get
            {
                tick_t end = 0;
                for (int i = 0; i < StreamCount; i++)
                {
                    var last = ObjectStreams[i].LastObject;
                    if (last != null)
                        end = end < last.EndPosition ? last.EndPosition : end;
                }
                return end;
            }
        }

        public time_t TimeEnd
        {
            get
            {
                time_t end = 0;
                for (int i = 0; i < StreamCount; i++)
                {
                    var last = ObjectStreams[i].LastObject;
                    if (last != null)
                        end = end < last.AbsoluteEndPosition ? last.AbsoluteEndPosition : end;
                }
                return end;
            }
        }

        private double? m_maxBpm = null;
        public double MaxBpm
        {
            get
            {
                if (!m_maxBpm.HasValue)
                {
                    double value = double.MinValue;
                    ControlPoints.ForEach(cp =>
                    {
                        if (cp.BeatsPerMinute > value)
                            value = cp.BeatsPerMinute;
                    });
                    m_maxBpm = value;
                }
                return m_maxBpm.Value;
            }
        }

        public Chart(int streamCount, GameMode gameMode = null)
        {
            StreamCount = streamCount;
            GameMode = gameMode;

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
        public void AddObject(ChartObject obj) => ObjectStreams[obj.Stream].Add(obj);

        public void ForEachObjectInRange(int stream, tick_t startPos, tick_t endPos, bool includeDuration, Action<ChartObject> action) =>
            this[stream].ForEachInRange(startPos, endPos, includeDuration, action);

        public void ForEachObjectInRange(int stream, time_t startPos, time_t endPos, bool includeDuration, Action<ChartObject> action) =>
            this[stream].ForEachInRange(startPos, endPos, includeDuration, action);

        public void ForEachObjectInRange(tick_t startPos, tick_t endPos, bool includeDuration, Action<ChartObject> action)
        {
            for (int i = 0; i < StreamCount; i++)
                this[i].ForEachInRange(startPos, endPos, includeDuration, action);
        }

        public void ForEachObjectInRange(time_t startPos, time_t endPos, bool includeDuration, Action<ChartObject> action)
        {
            for (int i = 0; i < StreamCount; i++)
                this[i].ForEachInRange(startPos, endPos, includeDuration, action);
        }

        public void ForEachControlPointInRange(int stream, tick_t startPos, tick_t endPos, Action<ControlPoint> action) =>
            ControlPoints.ForEachInRange(startPos, endPos, action);

        public void ForEachControlPointInRange(int stream, time_t startPos, time_t endPos, Action<ControlPoint> action) =>
            ControlPoints.ForEachInRange(startPos, endPos, action);

        public time_t CalcTimeFromTick(tick_t pos)
        {
            ControlPoint cp = ControlPoints.MostRecent(pos);
            return cp.AbsolutePosition + cp.MeasureDuration * (pos - cp.Position);
        }

        public sealed class ObjectStream : IEnumerable<ChartObject>
        {
            private readonly Chart m_chart;
            private readonly int m_stream;
            private readonly OrderedLinkedList<ChartObject> m_objects = new OrderedLinkedList<ChartObject>();

            public ChartObject FirstObject => m_objects.Count == 0 ? null : m_objects[0];
            public ChartObject LastObject => m_objects.Count == 0 ? null : m_objects[m_objects.Count - 1];
            
            public int Count => m_objects.Count;
            public ChartObject this[int index] => m_objects[index];

            internal ObjectStream(Chart chart, int stream)
            {
                m_chart = chart;
                m_stream = stream;
            }

            public IEnumerator<ChartObject> GetEnumerator() => m_objects.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => m_objects.GetEnumerator();

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
            public void Add(ChartObject obj)
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
            public ChartObject Add(tick_t position, tick_t duration = default) => Add<ChartObject>(position, duration);

            public T Add<T>(tick_t position, tick_t duration = default)
                where T : ChartObject, new()
            {
                var obj = new T()
                {
                    Position = position,
                    Duration = duration,
                };

                Add(obj);
                return obj;
            }

            /// <summary>
            /// If the object is contained in this stream, it is removed and
            ///  its Chart property is set to null.
            /// </summary>
            /// <param name="obj"></param>
            public void Remove(ChartObject obj)
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
            public ChartObject Find(tick_t position, bool includeDuration)
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

            public T Find<T>(tick_t position, bool includeDuration)
                where T : ChartObject
            {
                // TODO(local): make this a binary search?
                for (int i = 0, count = m_objects.Count; i < count; i++)
                {
                    var obj = m_objects[i];
                    if (includeDuration)
                    {
                        if (obj.Position <= position && obj.Duration >= position && obj is T t)
                            return t;
                    }
                    else if (obj.Position == position && obj is T t)
                        return t;
                }

                return null;
            }

            public ChartObject MostRecent(tick_t position)
            {
                for (int i = m_objects.Count - 1; i >= 0; i--)
                {
                    var obj = m_objects[i];
                    if (obj.Position <= position)
                        return obj;
                }
                return null;
            }

            public T MostRecent<T>(tick_t position)
                where T : ChartObject
            {
                for (int i = m_objects.Count - 1; i >= 0; i--)
                {
                    var obj = m_objects[i];
                    if (obj.Position <= position && obj is T t)
                        return t;
                }
                return null;
            }

            public ChartObject MostRecent(time_t position)
            {
                for (int i = m_objects.Count - 1; i >= 0; i--)
                {
                    var obj = m_objects[i];
                    if (obj.AbsolutePosition <= position)
                        return obj;
                }
                return null;
            }

            public T MostRecent<T>(time_t position)
                where T : ChartObject
            {
                for (int i = m_objects.Count - 1; i >= 0; i--)
                {
                    var obj = m_objects[i];
                    if (obj.AbsolutePosition <= position && obj is T t)
                        return t;
                }
                return null;
            }

            public void ForEach(Action<ChartObject> action)
            {
                if (action == null) return;
                for (int i = 0, count = m_objects.Count; i < count; i++)
                    action(m_objects[i]);
            }

            public void ForEach<T>(Action<T> action)
                where T : ChartObject
            {
                if (action == null) return;
                for (int i = 0, count = m_objects.Count; i < count; i++)
                    action((T)m_objects[i]);
            }

            public void ForEachInRange(tick_t startPos, tick_t endPos, bool includeDuration, Action<ChartObject> action)
            {
                if (action == null) return;

		        tick_t GetEndPosition(ChartObject obj)
		        {
			        if (includeDuration)
				        return obj.EndPosition;
			        else return obj.Position;
		        }
		
			    for (int i = 0; i < m_objects.Count; i++)
			    {
				    var obj = m_objects[i];
				    if (GetEndPosition(obj) < startPos)
					    continue;
				    if (obj.Position > endPos)
					    break;
				    action(obj);
			    }
            }

            public void ForEachInRange(time_t startPos, time_t endPos, bool includeDuration, Action<ChartObject> action)
            {
                if (action == null) return;

		        time_t GetEndPosition(ChartObject obj)
		        {
			        if (includeDuration)
				        return obj.AbsoluteEndPosition;
			        else return obj.AbsolutePosition;
		        }
		
			    for (int i = 0; i < m_objects.Count; i++)
			    {
				    var obj = m_objects[i];
				    if (GetEndPosition(obj) < startPos)
					    continue;
				    if (obj.AbsolutePosition > endPos)
					    break;
				    action(obj);
			    }
            }

            public bool TryGetAt(tick_t position, out ChartObject overlap)
            {
                overlap = null;
                for (int i = 0; i < m_objects.Count && overlap == null; i++)
                {
                    var obj = m_objects[i];
                    if (obj.Position == position)
                        overlap = obj;
                }
                return overlap != null;
            }
        }

        public sealed class ControlPointList : IEnumerable<ControlPoint>
        {
            private readonly Chart m_chart;
            private readonly OrderedLinkedList<ControlPoint> m_controlPoints = new OrderedLinkedList<ControlPoint>();

            public int Count => m_controlPoints.Count;
            public ControlPoint this[int index] => m_controlPoints[index];

            public ControlPoint Root => m_controlPoints[0];

            public double ModeBeatsPerMinute
            {
                get
                {
                    Dictionary<double, time_t> durations = new Dictionary<double, time_t>();

                    time_t chartEnd = m_chart.LastObjectTime;
                    time_t longestTime = -1;

                    double result = 120.0;
                    foreach (var point in m_controlPoints)
                    {
                        double bpm = point.BeatsPerMinute;
                        if (!durations.ContainsKey(bpm))
                            durations[bpm] = 0.0;

                        if (point.HasNext)
                            durations[bpm] += point.Next.AbsolutePosition - point.AbsolutePosition;
                        else durations[bpm] += chartEnd - point.AbsolutePosition;

                        if (durations[bpm] > longestTime)
                        {
                            longestTime = durations[bpm];
                            result = bpm;
                        }
                    }

                    return result;
                }
            }

            internal ControlPointList(Chart chart)
            {
                m_chart = chart;
                Add(new ControlPoint());
            }

            public IEnumerator<ControlPoint> GetEnumerator() => m_controlPoints.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => m_controlPoints.GetEnumerator();

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
                else result = new ControlPoint();
                result.Position = position;

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
                for (int i = m_controlPoints.Count - 1; i >= 0; i--)
                {
                    var cp = m_controlPoints[i];
                    if (cp.Position <= position)
                        return cp;
                }
                return null;
            }

            public ControlPoint MostRecent(time_t position)
            {
                for (int i = m_controlPoints.Count - 1; i >= 0; i--)
                {
                    var cp = m_controlPoints[i];
                    if (cp.AbsolutePosition <= position)
                        return cp;
                }
                return m_controlPoints[0];
            }

            public void ForEach(Action<ControlPoint> action)
            {
                if (action == null) return;
                for (int i = 0, count = m_controlPoints.Count; i < count; i++)
                    action(m_controlPoints[i]);
            }

            public void ForEachInRange(tick_t startPos, tick_t endPos, Action<ControlPoint> action)
            {
                if (action == null) return;

			    for (int i = 0; i < m_controlPoints.Count; i++)
			    {
				    var cp = m_controlPoints[i];
				    if (cp.Position < startPos)
					    continue;
				    if (cp.Position > endPos)
					    break;
				    action(cp);
			    }
            }

            public void ForEachInRange(time_t startPos, time_t endPos, Action<ControlPoint> action)
            {
                if (action == null) return;

			    for (int i = 0; i < m_controlPoints.Count; i++)
			    {
				    var cp = m_controlPoints[i];
				    if (cp.AbsolutePosition < startPos)
					    continue;
				    if (cp.AbsolutePosition > endPos)
					    break;
				    action(cp);
			    }
            }

            public ControlPoint FindAt(tick_t pos)
            {
			    for (int i = 0; i < m_controlPoints.Count; i++)
			    {
				    var cp = m_controlPoints[i];
                    if (cp.Position == pos)
                        return cp;
                }
                return null;
            }
        }
    }
}
