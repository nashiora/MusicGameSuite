using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRM
{
    public enum PlayDirection
    {
        Forward, Backward,
    }

    public class SlidingChartPlayback
    {
        public Chart Chart { get; }
        
        private time_t m_position = -9999;
        private time_t m_lookAhead = 1.0, m_lookBehind = 0.5;

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
        
        private List<Object>[] m_objsAhead, m_objsBehind;
        private List<Object>[] m_objsPrimary, m_objsSecondary;

        #region Granular Events
        
        public event Action<PlayDirection, Object> ObjectHeadCrossPrimary;
        public event Action<PlayDirection, Object> ObjectTailCrossPrimary;
        
        public event Action<PlayDirection, Object> ObjectHeadCrossCritical;
        public event Action<PlayDirection, Object> ObjectTailCrossCritical;
        
        public event Action<PlayDirection, Object> ObjectHeadCrossSecondary;
        public event Action<PlayDirection, Object> ObjectTailCrossSecondary;

        #endregion

        public SlidingChartPlayback(Chart chart)
        {
            m_objsAhead = new List<Object>[chart.StreamCount];
            m_objsAhead.Fill(i =>
            {
                var result = new List<Object>();
                chart[i].ForEach(obj => result.Add(obj));
                return result;
            });

            m_objsPrimary = new List<Object>[chart.StreamCount];
            m_objsPrimary.Fill(() => new List<Object>());

            m_objsSecondary = new List<Object>[chart.StreamCount];
            m_objsSecondary.Fill(() => new List<Object>());

            m_objsBehind = new List<Object>[chart.StreamCount];
            m_objsBehind.Fill(() => new List<Object>());

            Chart = chart;
        }

        private void SetNextPosition(time_t nextPos)
        {
            bool isForward = nextPos > m_position;
            m_position = nextPos;

            if (isForward)
            {
                void CheckEdge(time_t edge, List<Object>[] objsFrom, List<Object>[] objsTo, Action<PlayDirection, Object> headCross, Action<PlayDirection, Object> tailCross)
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

                CheckEdge(nextPos + LookAhead, m_objsAhead, m_objsPrimary, OnHeadCrossPrimary, OnTailCrossPrimary);
                CheckEdge(nextPos, m_objsPrimary, m_objsSecondary, OnHeadCrossCritical, OnTailCrossCritical);
                CheckEdge(nextPos - LookBehind, m_objsSecondary, m_objsBehind, OnHeadCrossSecondary, OnTailCrossSecondary);
            }
            else
            {
                // First, check for objects have passed the back cursor.
                // Second, check for objects which have passed the critical cursor.
                // Lastly, check for objects which have passed the front cursor.
            }
        }
        
        private void OnHeadCrossPrimary(PlayDirection dir, Object obj) => ObjectHeadCrossPrimary?.Invoke(dir, obj);
        private void OnTailCrossPrimary(PlayDirection dir, Object obj) => ObjectTailCrossPrimary?.Invoke(dir, obj);

        private void OnHeadCrossCritical(PlayDirection dir, Object obj) => ObjectHeadCrossCritical?.Invoke(dir, obj);
        private void OnTailCrossCritical(PlayDirection dir, Object obj) => ObjectTailCrossCritical?.Invoke(dir, obj);

        private void OnHeadCrossSecondary(PlayDirection dir, Object obj) => ObjectHeadCrossSecondary?.Invoke(dir, obj);
        private void OnTailCrossSecondary(PlayDirection dir, Object obj) => ObjectTailCrossSecondary?.Invoke(dir, obj);
    }
}
