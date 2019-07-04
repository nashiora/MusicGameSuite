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

        public Action<ChartObject> HeadCross;
        public Action<ChartObject> TailCross;

        internal List<ChartObject>[] m_objectsAhead;
        internal List<ChartObject>[] m_objectsBehind;

        public PlaybackWindow(string name, time_t where)
        {
            Name = name;
            Position = where;
        }

        internal void OnHeadCross(ChartObject obj)
        {
            HeadCross?.Invoke(obj);
        }

        internal void OnTailCross(ChartObject obj)
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

        private readonly List<PlaybackWindow> m_customWindows;

        private List<ChartObject>[] m_objsAhead, m_objsBehind;
        private List<ChartObject>[] m_objsPrimary, m_objsSecondary;

        #region Granular Events
        
        public event Action<PlayDirection, ChartObject> ObjectHeadCrossPrimary;
        public event Action<PlayDirection, ChartObject> ObjectTailCrossPrimary;
        
        public event Action<PlayDirection, ChartObject> ObjectHeadCrossCritical;
        public event Action<PlayDirection, ChartObject> ObjectTailCrossCritical;
        
        public event Action<PlayDirection, ChartObject> ObjectHeadCrossSecondary;
        public event Action<PlayDirection, ChartObject> ObjectTailCrossSecondary;

        #endregion

        public SlidingChartPlayback(Chart chart)
        {
            SetChart(chart);
        }

        public PlaybackWindow CreateWindow(string name, time_t where)
        {
            var window = new PlaybackWindow(name, where);
            if (Chart != null)
            {
                window.m_objectsAhead = new List<ChartObject>[Chart.StreamCount].Fill(i => new List<ChartObject>(Chart[i]));
                window.m_objectsBehind = new List<ChartObject>[Chart.StreamCount].Fill(() => new List<ChartObject>());
            }
            return window;
        }

        public void Reset()
        {
            SetChart(Chart);
        }

        public void SetChart(Chart chart)
        {
            if (chart == null) return;
            Chart = chart;

            m_position = -9999;

            m_objsAhead = new List<ChartObject>[Chart.StreamCount];
            m_objsAhead.Fill(i =>
            {
                var result = new List<ChartObject>(chart[i]);
                //chart[i].ForEach(obj => result.Add(obj));
                return result;
            });

            foreach (var window in m_customWindows)
            {
                window.m_objectsAhead = new List<ChartObject>[Chart.StreamCount].Fill(i => new List<ChartObject>(Chart[i]));
                window.m_objectsBehind = new List<ChartObject>[Chart.StreamCount].Fill(() => new List<ChartObject>());
            }

            m_objsPrimary = new List<ChartObject>[chart.StreamCount];
            m_objsPrimary.Fill(() => new List<ChartObject>());

            m_objsSecondary = new List<ChartObject>[chart.StreamCount];
            m_objsSecondary.Fill(() => new List<ChartObject>());

            m_objsBehind = new List<ChartObject>[chart.StreamCount];
            m_objsBehind.Fill(() => new List<ChartObject>());
        }

        private void SetNextPosition(time_t nextPos)
        {
            bool isForward = nextPos > m_position;
            m_position = nextPos;

            System.Diagnostics.Debug.Assert(isForward);

            if (isForward)
            {
                CheckEdgeForward(nextPos + LookAhead, m_objsAhead, m_objsPrimary, OnHeadCrossPrimary, OnTailCrossPrimary);
                CheckEdgeForward(nextPos, m_objsPrimary, m_objsSecondary, OnHeadCrossCritical, OnTailCrossCritical);
                CheckEdgeForward(nextPos - LookBehind, m_objsSecondary, m_objsBehind, OnHeadCrossSecondary, OnTailCrossSecondary);

                foreach (var window in m_customWindows)
                {
                    CheckEdgeForward(nextPos + window.Position, window.m_objectsAhead, window.m_objectsBehind,
                        (dir, obj) => window.OnHeadCross(obj), (dir, obj) => window.OnTailCross(obj));
                }
            }
            else
            {
                CheckEdgeBackward(nextPos - LookBehind, m_objsBehind, m_objsSecondary, OnHeadCrossSecondary, OnTailCrossSecondary);
                CheckEdgeBackward(nextPos, m_objsSecondary, m_objsPrimary, OnHeadCrossCritical, OnTailCrossCritical);
                CheckEdgeBackward(nextPos + LookAhead, m_objsPrimary, m_objsAhead, OnHeadCrossPrimary, OnTailCrossPrimary);
            }
        }

        private void CheckEdgeForward(time_t edge, List<ChartObject>[] objsFrom, List<ChartObject>[] objsTo, Action<PlayDirection, ChartObject> headCross, Action<PlayDirection, ChartObject> tailCross)
        {
            for (int stream = 0; stream < Chart.StreamCount; stream++)
            {
                var from = objsFrom[stream];
                for (int i = 0; i < from.Count; )
                {
                    var obj = from[i];
                    if (obj.AbsolutePosition < edge)
                    {
                        var to = objsTo[stream];
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
        
        private void CheckEdgeBackward(time_t edge, List<ChartObject>[] objsFrom, List<ChartObject>[] objsTo, Action<PlayDirection, ChartObject> headCross, Action<PlayDirection, ChartObject> tailCross)
        {
            for (int stream = 0; stream < Chart.StreamCount; stream++)
            {
                var from = objsFrom[stream];
                for (int i = 0; i < from.Count; )
                {
                    var obj = from[i];
                    if (obj.AbsoluteEndPosition > edge)
                    {
                        var to = objsTo[stream];
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
        
        public void AddObject(ChartObject obj)
        {
            List<ChartObject>[] CreateFake()
            {
                var fake = new List<ChartObject>[Chart.StreamCount];
                fake.Fill(() => new List<ChartObject>());
                return fake;
            }

            void TransferFake(List<ChartObject>[] fake, List<ChartObject>[] real)
            {
                for (int i = 0; i < Chart.StreamCount; i++)
                    real[i].AddRange(fake[i]);
            }
            
            List<ChartObject>[] fakeAhead = CreateFake();
            List<ChartObject>[] fakePrimary = CreateFake();
            List<ChartObject>[] fakeSecondary = CreateFake();
            List<ChartObject>[] fakeBehind = CreateFake();

            fakeAhead[obj.Stream].Add(obj);

            CheckEdgeForward(Position + LookAhead, fakeAhead, fakePrimary, OnHeadCrossPrimary, OnTailCrossPrimary);
            CheckEdgeForward(Position, fakePrimary, fakeSecondary, OnHeadCrossCritical, OnTailCrossCritical);
            CheckEdgeForward(Position - LookBehind, fakeSecondary, fakeBehind, OnHeadCrossSecondary, OnTailCrossSecondary);
            
            TransferFake(fakeAhead, m_objsAhead);
            TransferFake(fakePrimary, m_objsPrimary);
            TransferFake(fakeSecondary, m_objsSecondary);
            TransferFake(fakeBehind, m_objsBehind);
        }

        public void RemoveObject(ChartObject obj)
        {
            List<ChartObject>[] CreateFake()
            {
                var fake = new List<ChartObject>[Chart.StreamCount];
                fake.Fill(() => new List<ChartObject>());
                return fake;
            }

            void TransferReal(List<ChartObject>[] fake, List<ChartObject>[] real)
            {
                for (int i = 0; i < Chart.StreamCount; i++)
                {
                    if (real[i].Contains(obj))
                    {
                        real[i].Remove(obj);
                        fake[i].Add(obj);

                        return;
                    }
                }
            }
            
            List<ChartObject>[] fakeAhead = CreateFake();
            List<ChartObject>[] fakePrimary = CreateFake();
            List<ChartObject>[] fakeSecondary = CreateFake();
            List<ChartObject>[] fakeBehind = CreateFake();
            
            TransferReal(fakeAhead, m_objsAhead);
            TransferReal(fakePrimary, m_objsPrimary);
            TransferReal(fakeSecondary, m_objsSecondary);
            TransferReal(fakeBehind, m_objsBehind);

            CheckEdgeForward(Chart.TimeEnd + 1, fakeAhead, fakePrimary, OnHeadCrossPrimary, OnTailCrossPrimary);
            CheckEdgeForward(Chart.TimeEnd + 1, fakePrimary, fakeSecondary, OnHeadCrossCritical, OnTailCrossCritical);
            CheckEdgeForward(Chart.TimeEnd + 1, fakeSecondary, fakeBehind, OnHeadCrossSecondary, OnTailCrossSecondary);
        }

        private void OnHeadCrossPrimary(PlayDirection dir, ChartObject obj) => ObjectHeadCrossPrimary?.Invoke(dir, obj);
        private void OnTailCrossPrimary(PlayDirection dir, ChartObject obj) => ObjectTailCrossPrimary?.Invoke(dir, obj);

        private void OnHeadCrossCritical(PlayDirection dir, ChartObject obj) => ObjectHeadCrossCritical?.Invoke(dir, obj);
        private void OnTailCrossCritical(PlayDirection dir, ChartObject obj) => ObjectTailCrossCritical?.Invoke(dir, obj);

        private void OnHeadCrossSecondary(PlayDirection dir, ChartObject obj) => ObjectHeadCrossSecondary?.Invoke(dir, obj);
        private void OnTailCrossSecondary(PlayDirection dir, ChartObject obj) => ObjectTailCrossSecondary?.Invoke(dir, obj);
    }
}
