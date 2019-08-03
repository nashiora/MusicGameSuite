using System;
using System.Collections.Generic;

namespace theori.Charting.Playback
{
    public enum PlayDirection
    {
        Forward, Backward,
    }

    public sealed class PlaybackWindow
    {
        public readonly string Name;
        public time_t Position { get; internal set; }

        public Action<Entity> HeadCross;
        public Action<Entity> TailCross;

        internal Dictionary<LaneLabel, List<Entity>> m_objectsAhead;
        internal Dictionary<LaneLabel, List<Entity>> m_objectsBehind;

        public PlaybackWindow(string name, time_t where)
        {
            Name = name;
            Position = where;
        }

        internal void OnHeadCross(Entity obj)
        {
            HeadCross?.Invoke(obj);
        }

        internal void OnTailCross(Entity obj)
        {
            TailCross?.Invoke(obj);
        }
    }

    public class SlidingChartPlayback
    {
        public Chart Chart { get; private set; }
        
        private time_t m_position = -9999;
        private time_t m_lookAhead = 0.75, m_lookBehind = 0.5;

        public time_t Position
        {
            get => m_position;
            set => SetNextPosition(value);
        }

        public time_t LookAhead
        {
            get => m_lookAhead;
            set => m_lookAhead = value;
        }

        public time_t LookBehind
        {
            get => m_lookBehind;
            set => m_lookBehind = value;
        }

        public time_t TotalViewDuration => LookAhead + LookBehind;

        // <--0 behind--<  (|  <--1 sec--<  |  <--2 pri--<  |)  <--3 ahead--<<

        private readonly List<PlaybackWindow> m_customWindows = new List<PlaybackWindow>();

        private Dictionary<LaneLabel, List<Entity>> m_objsAhead, m_objsBehind;
        private Dictionary<LaneLabel, List<Entity>> m_objsPrimary, m_objsSecondary;

        #region Granular Events
        
        public event Action<PlayDirection, Entity> ObjectHeadCrossPrimary;
        public event Action<PlayDirection, Entity> ObjectTailCrossPrimary;
        
        public event Action<PlayDirection, Entity> ObjectHeadCrossCritical;
        public event Action<PlayDirection, Entity> ObjectTailCrossCritical;
        
        public event Action<PlayDirection, Entity> ObjectHeadCrossSecondary;
        public event Action<PlayDirection, Entity> ObjectTailCrossSecondary;

        #endregion

        public SlidingChartPlayback(Chart chart)
        {
            SetChart(chart);
        }

#if false
        public PlaybackWindow CreateWindow(string name, time_t where)
        {
            var window = new PlaybackWindow(name, where);
            if (Chart != null)
            {
                window.m_objectsAhead = new List<Entity>[Chart.LaneCount].Fill(i => new List<Entity>(Chart[i]));
                window.m_objectsBehind = new List<Entity>[Chart.LaneCount].Fill(() => new List<Entity>());
            }
            return window;
        }
#endif

        public void Reset()
        {
            SetChart(Chart);
        }

        public void SetChart(Chart chart)
        {
            if (chart == null) return;
            Chart = chart;

            m_position = -9999;

            Dictionary<LaneLabel, List<Entity>> CreateFilledObjs()
            {
                var result = new Dictionary<LaneLabel, List<Entity>>();
                foreach (var lane in chart.Lanes)
                    result[lane.Label] = new List<Entity>(lane);
                return result;
            }

            Dictionary<LaneLabel, List<Entity>> CreateObjs()
            {
                var result = new Dictionary<LaneLabel, List<Entity>>();
                foreach (var lane in chart.Lanes)
                    result[lane.Label] = new List<Entity>();
                return result;
            }

            m_objsAhead = CreateFilledObjs();

            foreach (var window in m_customWindows)
            {
                window.m_objectsAhead = CreateFilledObjs();
                window.m_objectsBehind = CreateObjs();
            }

            m_objsPrimary = CreateObjs();
            m_objsSecondary = CreateObjs();
            m_objsBehind = CreateObjs();
        }

        private void SetNextPosition(time_t nextPos)
        {
            if (nextPos == m_position) return;

            bool isForward = nextPos > m_position;
            m_position = nextPos;

            System.Diagnostics.Debug.Assert(isForward);

            if (isForward)
            {
                CheckEdgeForward(nextPos + LookAhead, m_objsAhead, m_objsPrimary, OnHeadCrossPrimary, OnTailCrossPrimary);
                CheckEdgeForward(nextPos, m_objsPrimary, m_objsSecondary, OnHeadCrossCritical, OnTailCrossCritical);
                CheckEdgeForward(nextPos - LookBehind, m_objsSecondary, m_objsBehind, OnHeadCrossSecondary, OnTailCrossSecondary);

#if false
                foreach (var window in m_customWindows)
                {
                    CheckEdgeForward(nextPos + window.Position, window.m_objectsAhead, window.m_objectsBehind,
                        (dir, obj) => window.OnHeadCross(obj), (dir, obj) => window.OnTailCross(obj));
                }
#endif
            }
            else
            {
                CheckEdgeBackward(nextPos - LookBehind, m_objsBehind, m_objsSecondary, OnHeadCrossSecondary, OnTailCrossSecondary);
                CheckEdgeBackward(nextPos, m_objsSecondary, m_objsPrimary, OnHeadCrossCritical, OnTailCrossCritical);
                CheckEdgeBackward(nextPos + LookAhead, m_objsPrimary, m_objsAhead, OnHeadCrossPrimary, OnTailCrossPrimary);
            }
        }

        private void CheckEdgeForward(time_t edge, Dictionary<LaneLabel, List<Entity>> objsFrom, Dictionary<LaneLabel, List<Entity>> objsTo, Action<PlayDirection, Entity> headCross, Action<PlayDirection, Entity> tailCross)
        {
            foreach (var (label, from) in objsFrom)
            {
                for (int i = 0; i < from.Count; )
                {
                    var obj = from[i];
                    if (obj.AbsolutePosition < edge)
                    {
                        var to = objsTo[label];
                        if (!to.Contains(obj))
                        {
                            // entered the seconary section, passed the critical-edge.
                            to.Add(obj);
                            headCross(PlayDirection.Forward, obj);
                        }

                        if (obj.AbsoluteEndPosition < edge)
                        {
                            // completely passed the critical-edge, now only in the secondary section.
                            from.RemoveAt(i);
                            tailCross(PlayDirection.Forward, obj);
                                
                            // don't increment `i` if we removed something
                            continue;
                        }
                    }
                    i++;
                }
            }
        }
        
        private void CheckEdgeBackward(time_t edge, Dictionary<LaneLabel, List<Entity>> objsFrom, Dictionary<LaneLabel, List<Entity>> objsTo, Action<PlayDirection, Entity> headCross, Action<PlayDirection, Entity> tailCross)
        {
            foreach (var (label, from) in objsFrom)
            {
                for (int i = 0; i < from.Count; )
                {
                    var obj = from[i];
                    if (obj.AbsoluteEndPosition > edge)
                    {
                        var to = objsTo[label];
                        if (!to.Contains(obj))
                        {
                            // entered the seconary section, passed the critical-edge.
                            to.Add(obj);
                            tailCross(PlayDirection.Backward, obj);
                        }

                        if (obj.AbsolutePosition > edge)
                        {
                            // completely passed the critical-edge, now only in the secondary section.
                            from.RemoveAt(i);
                            headCross(PlayDirection.Forward, obj);
                                
                            // don't increment `i` if we removed something
                            continue;
                        }
                    }
                    i++;
                }
            }
        }

#if false
        public void AddObject(Entity obj)
        {
            List<Entity>[] CreateFake()
            {
                var fake = new List<Entity>[Chart.LaneCount];
                fake.Fill(() => new List<Entity>());
                return fake;
            }

            void TransferFake(List<Entity>[] fake, List<Entity>[] real)
            {
                for (int i = 0; i < Chart.LaneCount; i++)
                    real[i].AddRange(fake[i]);
            }
            
            List<Entity>[] fakeAhead = CreateFake();
            List<Entity>[] fakePrimary = CreateFake();
            List<Entity>[] fakeSecondary = CreateFake();
            List<Entity>[] fakeBehind = CreateFake();

            fakeAhead[obj.Stream].Add(obj);

            CheckEdgeForward(Position + LookAhead, fakeAhead, fakePrimary, OnHeadCrossPrimary, OnTailCrossPrimary);
            CheckEdgeForward(Position, fakePrimary, fakeSecondary, OnHeadCrossCritical, OnTailCrossCritical);
            CheckEdgeForward(Position - LookBehind, fakeSecondary, fakeBehind, OnHeadCrossSecondary, OnTailCrossSecondary);
            
            TransferFake(fakeAhead, m_objsAhead);
            TransferFake(fakePrimary, m_objsPrimary);
            TransferFake(fakeSecondary, m_objsSecondary);
            TransferFake(fakeBehind, m_objsBehind);
        }

        public void RemoveObject(Entity obj)
        {
            List<Entity>[] CreateFake()
            {
                var fake = new List<Entity>[Chart.LaneCount];
                fake.Fill(() => new List<Entity>());
                return fake;
            }

            void TransferReal(List<Entity>[] fake, List<Entity>[] real)
            {
                for (int i = 0; i < Chart.LaneCount; i++)
                {
                    if (real[i].Contains(obj))
                    {
                        real[i].Remove(obj);
                        fake[i].Add(obj);

                        return;
                    }
                }
            }
            
            List<Entity>[] fakeAhead = CreateFake();
            List<Entity>[] fakePrimary = CreateFake();
            List<Entity>[] fakeSecondary = CreateFake();
            List<Entity>[] fakeBehind = CreateFake();
            
            TransferReal(fakeAhead, m_objsAhead);
            TransferReal(fakePrimary, m_objsPrimary);
            TransferReal(fakeSecondary, m_objsSecondary);
            TransferReal(fakeBehind, m_objsBehind);

            CheckEdgeForward(Chart.TimeEnd + 1, fakeAhead, fakePrimary, OnHeadCrossPrimary, OnTailCrossPrimary);
            CheckEdgeForward(Chart.TimeEnd + 1, fakePrimary, fakeSecondary, OnHeadCrossCritical, OnTailCrossCritical);
            CheckEdgeForward(Chart.TimeEnd + 1, fakeSecondary, fakeBehind, OnHeadCrossSecondary, OnTailCrossSecondary);
        }
#endif

        private void OnHeadCrossPrimary(PlayDirection dir, Entity obj) => ObjectHeadCrossPrimary?.Invoke(dir, obj);
        private void OnTailCrossPrimary(PlayDirection dir, Entity obj) => ObjectTailCrossPrimary?.Invoke(dir, obj);

        private void OnHeadCrossCritical(PlayDirection dir, Entity obj) => ObjectHeadCrossCritical?.Invoke(dir, obj);
        private void OnTailCrossCritical(PlayDirection dir, Entity obj) => ObjectTailCrossCritical?.Invoke(dir, obj);

        private void OnHeadCrossSecondary(PlayDirection dir, Entity obj) => ObjectHeadCrossSecondary?.Invoke(dir, obj);
        private void OnTailCrossSecondary(PlayDirection dir, Entity obj) => ObjectTailCrossSecondary?.Invoke(dir, obj);
    }
}
