using OpenRM;

namespace NeuroSonic.GamePlay.Scoring
{
    public abstract class StreamJudge
    {
        public Chart Chart { get; }
        public int StreamIndex { get; }
        public Chart.ObjectStream Objects => Chart[StreamIndex];

        protected abstract time_t JudgementRadius { get; }

        protected time_t CurrentPosition { get; private set; }

        private Object m_mostRecentInactive;
        private Object m_firstActive, m_lastActive;

        protected StreamJudge(Chart chart, int streamIndex)
        {
            Chart = chart;
            StreamIndex = streamIndex;
        }

        internal void InternalAdvancePosition(time_t position)
        {
            CurrentPosition = position;

            // This function assumes that everything is stored in order.
            // Doing so means we don't need to allocate a list, just store the first
            //  and last objects inside the window and search forward.

            time_t frontEdge = position + JudgementRadius;
            time_t backEdge = position - JudgementRadius;

            if (m_firstActive == null)
            {
                Object first;
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
        }

        protected abstract void ObjectEnteredJudgement(Object obj);
        protected abstract void ObjectExitedJudgement(Object obj);
    }
}
