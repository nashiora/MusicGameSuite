using theori;

using OpenRM;

namespace NeuroSonic.GamePlay.Scoring
{
    public sealed class MasterJudge
    {
        public Chart Chart { get; }

        private time_t m_position = double.MinValue;
        public time_t Position
        {
            get => m_position;
            set
            {
                if (value < m_position)
                    throw new System.Exception("Cannot rewind score judgement");
                else if (value == m_position) return;

                m_position = value;
                AdvancePosition(value);
            }
        }

        public int Score { get; private set; }

        private readonly StreamJudge[] m_judges = new StreamJudge[8];

        public StreamJudge this[int index] => m_judges[index];

        public MasterJudge(Chart chart)
        {
            Chart = chart;

            for (int i = 0; i < 6; i++)
                m_judges[i] = new ButtonJudge(chart, i);
        }

        private void AdvancePosition(time_t position)
        {
            for (int i = 0; i < 8; i++)
            {
                var judge = m_judges[i];
                if (judge != null)
                    judge.InternalAdvancePosition(position);
            }
        }
    }
}
