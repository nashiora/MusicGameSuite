using System;
using System.Collections;
using System.Collections.Generic;

using theori.GameModes;

namespace theori.Charting
{
    [Flags]
    public enum EntityRelation
    {
        /// <summary>
        /// Specifies that there is no required relation between two entity types.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that the given entity type must be the same as the required entity type.
        /// </summary>
        Equal = 0b01,
        /// <summary>
        /// Specifies that the given entity type must be a subclass of the required entity type.
        /// </summary>
        Subclass = 0b10,

        /// <summary>
        /// Specifies that the given entity type must be equal to or a subclass of the required entity type.
        /// </summary>
        EqualOrSubclass = Equal | Subclass,
    }

    public struct LaneLabel : IEquatable<LaneLabel>
    {
        public enum Kind
        {
            Text, Number
        }

        public static implicit operator LaneLabel(string text) => new LaneLabel(text);
        public static implicit operator LaneLabel(int number) => new LaneLabel(number);

        public static explicit operator int(LaneLabel label)
        {
            if (label.LabelKind != Kind.Number)
                throw new ArgumentException("Label was not numeric.", nameof(label));
            return label.m_num;
        }

        public static explicit operator string(LaneLabel label)
        {
            if (label.LabelKind != Kind.Text)
                throw new ArgumentException("Label was not textual.", nameof(label));
            return label.m_text;
        }

        public static bool operator ==(LaneLabel a, LaneLabel b) => a.Equals(b);
        public static bool operator !=(LaneLabel a, LaneLabel b) => !a.Equals(b);

        public static bool operator ==(LaneLabel a, string b) => a.LabelKind == Kind.Text && a.m_text == b;
        public static bool operator !=(LaneLabel a, string b) => a.LabelKind != Kind.Text || a.m_text != b;

        public static bool operator ==(LaneLabel a, int b) => a.LabelKind == Kind.Number && a.m_num == b;
        public static bool operator !=(LaneLabel a, int b) => a.LabelKind != Kind.Number || a.m_num != b;

        private readonly string m_text;
        private readonly int m_num;

        private readonly int m_hashCode;

        public readonly Kind LabelKind;

        public LaneLabel(string text)
        {
            LabelKind = Kind.Text;
            m_text = text ?? throw new ArgumentNullException(nameof(text));
            m_hashCode = m_text.GetHashCode();

            m_num = 0;
        }

        public LaneLabel(int num)
        {
            LabelKind = Kind.Number;
            m_num = num;
            m_hashCode = num;

            m_text = null;
        }

        public override bool Equals(object obj) => obj is LaneLabel label && Equals(label);
        public bool Equals(LaneLabel that)
        {
            if (LabelKind != that.LabelKind) return false;
            return LabelKind == Kind.Text ? m_text == that.m_text : m_num == that.m_num;
        }

        public override int GetHashCode() => m_hashCode;

        public override string ToString() => LabelKind == Kind.Text ? m_text : m_num.ToString();
    }

    /// <summary>
    /// Contains all relevant data for a single chart.
    /// </summary>
    public sealed class Chart
    {
        public ChartSetInfo SetInfo => Info.Set;
        public ChartInfo Info { get; set; } = new ChartInfo();

        public readonly GameMode GameMode;

        private readonly Dictionary<LaneLabel, ChartLane> m_lanes = new Dictionary<LaneLabel, ChartLane>();
        public IEnumerable<ChartLane> Lanes => m_lanes.Values;

        public readonly ControlPointList ControlPoints;

        public time_t LastObjectTime
        {
            get
            {
                time_t lastTime = double.MinValue;
                foreach (var lane in Lanes)
                {
                    if (lane.Count == 0) continue;

                    time_t t = lane.Last.AbsolutePosition;
                    if (t > lastTime)
                        lastTime = t;
                }
                return lastTime;
            }
        }

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
                foreach (var lane in Lanes)
                {
                    var last = lane.Last;
                    if (last != null)
                        end = end < last.EndPosition ? last.EndPosition : end;
                }
                return end;
            }
        }

        public time_t TimeStart
        {
            get
            {
                time_t start = double.MaxValue;
                foreach (var lane in Lanes)
                {
                    var last = lane.First;
                    if (last != null)
                        start = start > last.AbsolutePosition ? last.AbsolutePosition : start;
                }
                return start;
            }
        }

        public time_t TimeEnd
        {
            get
            {
                time_t end = 0;
                foreach (var lane in Lanes)
                {
                    var last = lane.Last;
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

        public Chart(GameMode gameMode = null)
        {
            GameMode = gameMode;
            ControlPoints = new ControlPointList(this);
        }

        public Chart(int laneCount, GameMode gameMode = null)
        {
            GameMode = gameMode;
            ControlPoints = new ControlPointList(this);

            for (int i = 0; i < laneCount; i++)
                CreateOpenLane(i);
        }

        #region Lane Creation

        private void ThrowIfLaneExists(LaneLabel name)
        {
            if (m_lanes.ContainsKey(name))
                throw new ArgumentException(nameof(name));
        }

        public ChartLane CreateOpenLane(LaneLabel name)
        {
            ThrowIfLaneExists(name);
            return m_lanes[name] = new ChartLane(this, name);
        }

        public ChartLane CreateTypedLane<TEntity>(LaneLabel name, EntityRelation relation = EntityRelation.Equal)
            where TEntity : Entity
        {
            return CreateTypedLane(name, typeof(TEntity), relation);
        }

        public ChartLane CreateTypedLane(LaneLabel name, Type type, EntityRelation relation = EntityRelation.Equal)
        {
            ThrowIfLaneExists(name);
            return m_lanes[name] = new ChartLane(this, name, new[] { type }, relation);
        }

        public ChartLane CreateMultiTypedLane<TEntity0, TEntity1>(LaneLabel name, EntityRelation relation = EntityRelation.Equal)
            where TEntity0 : Entity
            where TEntity1 : Entity
        {
            return CreateMultiTypedLane(name, new[] { typeof(TEntity0), typeof(TEntity1) }, relation);
        }

        public ChartLane CreateMultiTypedLane<TEntity0, TEntity1, TEntity2>(LaneLabel name, EntityRelation relation = EntityRelation.Equal)
            where TEntity0 : Entity
            where TEntity1 : Entity
            where TEntity2 : Entity
        {
            return CreateMultiTypedLane(name, new[] { typeof(TEntity0), typeof(TEntity1), typeof(TEntity2) }, relation);
        }

        public ChartLane CreateMultiTypedLane<TEntity0, TEntity1, TEntity2, TEntity3>(LaneLabel name, EntityRelation relation = EntityRelation.Equal)
            where TEntity0 : Entity
            where TEntity1 : Entity
            where TEntity2 : Entity
            where TEntity3 : Entity
        {
            return CreateMultiTypedLane(name, new[] { typeof(TEntity0), typeof(TEntity1), typeof(TEntity2), typeof(TEntity3) }, relation);
        }

        public ChartLane CreateMultiTypedLane<TEntity0, TEntity1, TEntity2, TEntity3, TEntity4>(LaneLabel name, EntityRelation relation = EntityRelation.Equal)
            where TEntity0 : Entity
            where TEntity1 : Entity
            where TEntity2 : Entity
            where TEntity3 : Entity
            where TEntity4 : Entity
        {
            return CreateMultiTypedLane(name, new[] { typeof(TEntity0), typeof(TEntity1), typeof(TEntity2), typeof(TEntity3), typeof(TEntity4) }, relation);
        }

        public ChartLane CreateMultiTypedLane(LaneLabel name, Type[] types, EntityRelation relation = EntityRelation.Equal)
        {
            ThrowIfLaneExists(name);
            return m_lanes[name] = new ChartLane(this, name, types, relation);
        }

        #endregion

        #region Lane Getting

        public ChartLane this[LaneLabel name] => GetLane(name);
        public ChartLane GetLane(LaneLabel name) => m_lanes[name];

        #endregion

        internal void InvalidateTimeCalc()
        {
            foreach (var lane in Lanes)
                lane.InvalidateTimeCalc();
                
            ControlPoints.InvalidateTimeCalc();
        }

        public time_t CalcTimeFromTick(tick_t pos)
        {
            ControlPoint cp = ControlPoints.MostRecent(pos);
            return cp.AbsolutePosition + cp.MeasureDuration * (pos - cp.Position);
        }

        public sealed class ChartLane : IEnumerable<Entity>
        {
            private readonly Chart m_chart;
            public readonly LaneLabel Label;

            private readonly OrderedLinkedList<Entity> m_entities = new OrderedLinkedList<Entity>();

            private readonly Type[] m_allowedTypes;
            public readonly EntityRelation Relation;

            public IEnumerable<Type> AllowedTypes => m_allowedTypes;

            public Entity First => m_entities.Count == 0 ? null : m_entities[0];
            public Entity Last => m_entities.Count == 0 ? null : m_entities[m_entities.Count - 1];
            
            public int Count => m_entities.Count;
            public Entity this[int index] => m_entities[index];

            internal ChartLane(Chart chart, LaneLabel name)
            {
                m_chart = chart;
                Label = name;
                Relation = EntityRelation.None;
            }

            internal ChartLane(Chart chart, LaneLabel name, Type[] types, EntityRelation relation)
            {
                m_chart = chart;
                Label = name;
                m_allowedTypes = types;
                Relation = relation;
            }

            public IEnumerator<Entity> GetEnumerator() => m_entities.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => m_entities.GetEnumerator();

            internal void InvalidateTimeCalc()
            {
                foreach (var obj in m_entities) obj.InvalidateTimingCalc();
            }

            private void ValidateTypeRequirement(Type requestedType)
            {
                if (requestedType != typeof(Entity) && !requestedType.IsSubclassOf(typeof(Entity)))
                    throw new ChartFormatException($"{ requestedType } does not inherit from { nameof(Entity) }: cannot add to { nameof(ChartLane) }");

                if (Relation == EntityRelation.None)
                    return; // always allow any type when no relation is specified

                bool isAllowed = false;
                foreach (var allowedType in m_allowedTypes)
                {
                    if (isAllowed) break;

                    if ((Relation & EntityRelation.Equal) != 0)
                    {
                        if (allowedType == requestedType)
                            isAllowed = true;
                    }
                    else if ((Relation & EntityRelation.Subclass) != 0)
                    {
                        if (requestedType.IsSubclassOf(allowedType))
                            isAllowed = true;
                    }
                }

                if (!isAllowed)
                {
                    string message;
                    if (Label != null)
                        message = $"{ nameof(ChartLane) } (\"{ Label }\") does not allow entities of type { requestedType } ({ Entity.GetEntityIdByType(requestedType) }).";
                    else message = $"{ nameof(ChartLane) } does not allow entities of type { requestedType } ({ Entity.GetEntityIdByType(requestedType) }).";

                    throw new ChartFormatException(message);
                }
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
            /// This function adds the given object to this stream and assigns.
            ///  the object's Chart and Stream fields accordingly.
            /// </summary>
            public void Add(Entity obj)
            {
                System.Diagnostics.Debug.Assert(obj.Position >= 0);
                ValidateTypeRequirement(obj.GetType());

                // already added in the correct place
                if (obj.Chart == m_chart && obj.Lane == Label)
                    return;

                if (obj.Chart != null)
                {
                    if (obj.Chart != m_chart)
                        throw new ArgumentException("Given object is parented to a different chart!.", nameof(obj));
                    m_chart[obj.Lane].Remove(obj);
                }

                obj.Chart = m_chart;
                obj.m_lane = Label;

                m_entities.Add(obj);
            }

            /// <summary>
            /// Constructs a new Object and calls `Add(Object)` to add it to this stream.
            /// The newly created Object is returned.
            /// </summary>
            public Entity Add(tick_t position, tick_t duration = default) => Add<Entity>(position, duration);

            public T Add<T>(tick_t position, tick_t duration = default)
                where T : Entity, new()
            {
                ValidateTypeRequirement(typeof(T));

                var obj = new T()
                {
                    Position = position,
                    Duration = duration,
                };

                Add(obj);
                return obj;
            }

            public Entity Add(Type entityType, tick_t position, tick_t duration = default)
            {
                ValidateTypeRequirement(entityType);

                var obj = (Entity)Activator.CreateInstance(entityType);
                obj.Position = position;
                obj.Duration = duration;

                Add(obj);
                return obj;
            }

            /// <summary>
            /// If the object is contained in this stream, it is removed and
            ///  its Chart property is set to null.
            /// </summary>
            /// <param name="obj"></param>
            public void Remove(Entity obj)
            {
                if (obj.Chart == m_chart && obj.Lane == Label)
                {
                    bool rem = m_entities.Remove(obj);
                    System.Diagnostics.Debug.Assert(rem);
                    obj.Chart = null;
                }
            }

            /// <summary>
            /// Find the object, if any, at the current position.
            /// If `includeDuration` is true, the first object which contains
            ///  the given position is returned.
            /// </summary>
            public Entity Find(tick_t position, bool includeDuration)
            {
                // TODO(local): make this a binary search?
                for (int i = 0, count = m_entities.Count; i < count; i++)
                {
                    var obj = m_entities[i];
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
                where T : Entity
            {
                // TODO(local): make this a binary search?
                for (int i = 0, count = m_entities.Count; i < count; i++)
                {
                    var obj = m_entities[i];
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

            public Entity MostRecent(tick_t position)
            {
                for (int i = m_entities.Count - 1; i >= 0; i--)
                {
                    var obj = m_entities[i];
                    if (obj.Position <= position)
                        return obj;
                }
                return null;
            }

            public T MostRecent<T>(tick_t position)
                where T : Entity
            {
                for (int i = m_entities.Count - 1; i >= 0; i--)
                {
                    var obj = m_entities[i];
                    if (obj.Position <= position && obj is T t)
                        return t;
                }
                return null;
            }

            public Entity MostRecent(time_t position)
            {
                for (int i = m_entities.Count - 1; i >= 0; i--)
                {
                    var obj = m_entities[i];
                    if (obj.AbsolutePosition <= position)
                        return obj;
                }
                return null;
            }

            public T MostRecent<T>(time_t position)
                where T : Entity
            {
                for (int i = m_entities.Count - 1; i >= 0; i--)
                {
                    var obj = m_entities[i];
                    if (obj.AbsolutePosition <= position && obj is T t)
                        return t;
                }
                return null;
            }

            public void ForEach(Action<Entity> action)
            {
                if (action == null) return;
                for (int i = 0, count = m_entities.Count; i < count; i++)
                    action(m_entities[i]);
            }

            public void ForEach<T>(Action<T> action)
                where T : Entity
            {
                if (action == null) return;
                for (int i = 0, count = m_entities.Count; i < count; i++)
                    action((T)m_entities[i]);
            }

            public void ForEachInRange(tick_t startPos, tick_t endPos, bool includeDuration, Action<Entity> action)
            {
                if (action == null) return;

		        tick_t GetEndPosition(Entity obj)
		        {
			        if (includeDuration)
				        return obj.EndPosition;
			        else return obj.Position;
		        }
		
			    for (int i = 0; i < m_entities.Count; i++)
			    {
				    var obj = m_entities[i];
				    if (GetEndPosition(obj) < startPos)
					    continue;
				    if (obj.Position > endPos)
					    break;
				    action(obj);
			    }
            }

            public void ForEachInRange(time_t startPos, time_t endPos, bool includeDuration, Action<Entity> action)
            {
                if (action == null) return;

		        time_t GetEndPosition(Entity obj)
		        {
			        if (includeDuration)
				        return obj.AbsoluteEndPosition;
			        else return obj.AbsolutePosition;
		        }
		
			    for (int i = 0; i < m_entities.Count; i++)
			    {
				    var obj = m_entities[i];
				    if (GetEndPosition(obj) < startPos)
					    continue;
				    if (obj.AbsolutePosition > endPos)
					    break;
				    action(obj);
			    }
            }

            public bool TryGetAt(tick_t position, out Entity overlap)
            {
                overlap = null;
                for (int i = 0; i < m_entities.Count && overlap == null; i++)
                {
                    var obj = m_entities[i];
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
