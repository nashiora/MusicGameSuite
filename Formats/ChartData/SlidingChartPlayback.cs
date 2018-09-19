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
        
        private time_t m_position = double.MinValue;
        private time_t m_lookAhead = 1.0, m_lookBehind = 0.2;

        public time_t Position
        {
            get => m_position;
            set => SetNextPosition(value);
        }

        public time_t LookAhead { get; set; }
        public time_t LookBehind { get; set; }

        public time_t TotalViewDuration => LookAhead + LookBehind;

        // <--0 behind--<  (|  <--1 sec--<  |  <--2 pri--<  |)  <--3 ahead--<<

        private List<Object>[] m_objsPrimary, m_objsSecondary;

        public SlidingChartPlayback(Chart chart)
        {
            Chart = chart;
            Position = chart.Offset;
        }

        private void SetNextPosition(time_t nextPos)
        {
            bool isForward = nextPos > m_position;

            if (isForward)
            {
                // First, check for objects have passed the front cursor.
                time_t lastAheadEdge = m_position + LookAhead;
                time_t newAheadEdge = nextPos + LookAhead;

                Chart.ForEachObjectInRange(lastAheadEdge, newAheadEdge, false, obj =>
                {
                    int stream = obj.Stream;
                    // this object is already in the primary group, which should mean it lies on one of
                    //  the above boundaries. (can't easily look ahead by 1/inf seconds to skip repeats).
                    if (m_objsPrimary[stream].Contains(obj)) return; // in a lamda, no break/continue

                    m_objsPrimary[stream].Add(obj);
                    OnHeadCrossPrimary(PlayDirection.Forward, obj);
                });

                // Second, check for objects which have passed the critical cursor.
                // These can only come from the primary section

                // Really this step is processing everything in the Primary section.

                time_t newCriticalEdge = nextPos;

                for (int stream = 0; stream < Chart.StreamCount; stream++)
                {
                    var primary = m_objsPrimary[stream];
                    for (int i = 0; i < primary.Count; )
                    {
                        var obj = primary[i];
                        if (obj.AbsoluteEndPosition < newAheadEdge)
                        {
                            // completely passed the ahead-edge.
                            OnTailCrossPrimary(PlayDirection.Forward, obj);
                        }

                        if (obj.AbsolutePosition <= newCriticalEdge)
                        {
                            var secondary = m_objsSecondary[stream];
                            if (!secondary.Contains(obj))
                            {
                                // entered the seconary section, passed the critical-edge.
                                m_objsSecondary[stream].Add(obj);
                                OnHeadCrossCritical(PlayDirection.Forward, obj);
                            }

                            if (obj.AbsoluteEndPosition < newCriticalEdge)
                            {
                                // completely passed the critical-edge, now only in the secondary section.
                                primary.RemoveAt(i);
                                OnTailCrossCritical(PlayDirection.Forward, obj);

                                continue;
                            }
                        }
                        // don't increment `i` if we removed something
                        else i++;
                    }
                }

                // Lastly, check for objects which have passed the back cursor.
                // These can only come from the secondary section

                time_t newBehindEdge = nextPos;

                for (int stream = 0; stream < Chart.StreamCount; stream++)
                {
                    var secondary = m_objsSecondary[stream];
                    for (int i = 0; i < secondary.Count; )
                    {
                        var obj = secondary[i];
                        if (obj.AbsolutePosition <= newBehindEdge)
                        {
                            secondary.RemoveAt(i);

                            //OnCrossSecondary(PlayDirection.Forward, obj);
                        }
                        // don't increment `i` if we removed something
                        else i++;
                    }
                }
            }
            else
            {
                // First, check for objects have passed the back cursor.
                // Second, check for objects which have passed the critical cursor.
                // Lastly, check for objects which have passed the front cursor.
            }
        }
        
        private void OnHeadCrossPrimary(PlayDirection dir, Object obj) { }
        private void OnTailCrossPrimary(PlayDirection dir, Object obj) { }

        private void OnHeadCrossCritical(PlayDirection dir, Object obj) { }
        private void OnTailCrossCritical(PlayDirection dir, Object obj) { }

        private void OnHeadCrossSecondary(PlayDirection dir, Object obj) { }
        private void OnTailCrossSecondary(PlayDirection dir, Object obj) { }
    }
}
