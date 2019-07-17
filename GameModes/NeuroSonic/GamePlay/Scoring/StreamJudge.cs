using theori;
using theori.Charting;

namespace NeuroSonic.GamePlay.Scoring
{
    public abstract class StreamJudge
    {
        public Chart Chart { get; }
        public int StreamIndex { get; }
        public Chart.ObjectStream Objects => Chart[StreamIndex];

        protected abstract time_t JudgementRadius { get; }

        protected time_t CurrentPosition { get; private set; }

        private ChartObject m_mostRecentInactive;
        private ChartObject m_firstActive, m_lastActive;

        private bool m_completed = false;

        /// <summary>
        /// This is the "Input Offset" of the system.
        /// 
        /// A positive value will require the player to hit EARLIER.
        /// A negative value will then require the player to hit LATER.
        /// 
        /// 
        /// </summary>
        public time_t JudgementOffset = 0.0;

        public bool AutoPlay = false;

        protected StreamJudge(Chart chart, int streamIndex)
        {
            Chart = chart;
            StreamIndex = streamIndex;

            if (Objects.Count == 0)
                m_completed = true;
        }

        internal void InternalAdvancePosition(time_t position)
        {
            CurrentPosition = position;

            if (!m_completed)
            {
                // This function assumes that everything is stored in order.
                // Doing so means we don't need to allocate a list, just store the first
                //  and last objects inside the window and search forward.

                time_t frontEdge = position + JudgementRadius + JudgementOffset;
                time_t backEdge = position - JudgementRadius + JudgementOffset;

                if (m_firstActive == null)
                {
                    ChartObject first;
                    if (m_mostRecentInactive == null)
                        first = Objects.FirstObject;
                    else first = m_mostRecentInactive.Next;

                    // activate an object!
                    if (first.AbsolutePosition <= frontEdge)
                    {
                        m_firstActive = m_lastActive = first;
                        ObjectEnteredJudgement(m_firstActive);
                    }
                }

                var nextCheck = m_lastActive?.Next;
                while (nextCheck != null && nextCheck.AbsolutePosition <= frontEdge)
                {
                    m_lastActive = nextCheck;
                    ObjectEnteredJudgement(m_lastActive);

                    nextCheck = nextCheck.Next;
                }

                // now handle leaving the window

                while (m_firstActive != null)
                {
                    // very important, this assumes all objects end by or before the time the next one starts!!
                    if (m_firstActive.AbsoluteEndPosition > backEdge) break;

                    m_mostRecentInactive = m_firstActive;
                    ObjectExitedJudgement(m_firstActive);

                    if (m_firstActive == m_lastActive)
                        m_firstActive = m_lastActive = null;
                    else m_firstActive = m_firstActive.Next;
                }

                if (m_mostRecentInactive != null && m_mostRecentInactive.Next == null)
                    m_completed = true;
            }

            AdvancePosition(position);
        }

        protected abstract void AdvancePosition(time_t position);
        public abstract int CalculateNumScorableTicks();
        protected abstract void ObjectEnteredJudgement(ChartObject obj);
        protected abstract void ObjectExitedJudgement(ChartObject obj);
    }
}
