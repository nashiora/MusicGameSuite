using System;
using System.Collections.Generic;

using OpenRM;

namespace NeuroSonic.GamePlay.Scoring
{
    public class LaserJudge : StreamJudge
    {
        public struct Tick
        {
            public OpenRM.Object AssociatedObject;

            public time_t Position;
            public bool IsSegment;

            public bool IsAutoTick;

            public Tick(OpenRM.Object obj, time_t pos, bool isSegment, bool isAutoTick = false)
            {
                AssociatedObject = obj;
                Position = pos;
                IsSegment = isSegment;

                IsAutoTick = isAutoTick;
            }
        }

        public const int TOTAL_LASER_MILLIS = 72 * 2;

        private const double LASER_RADIUS = (TOTAL_LASER_MILLIS / 2) / 1000.0;

        private const double MAX_RADIUS = LASER_RADIUS;

        private bool m_userInputed = false;
        private time_t m_userWhen = 0.0;

        private readonly List<Tick> m_ticks = new List<Tick>();

        public event Action<time_t, OpenRM.Object> OnSlamHit;

        public event Action<time_t, OpenRM.Object> OnLaserActivated;
        public event Action<time_t, OpenRM.Object> OnLaserDeactivated;

        public event Action<OpenRM.Object, time_t, JudgeResult> OnTickProcessed;

        public LaserJudge(Chart chart, int streamIndex)
            : base(chart, streamIndex)
        {
        }

        protected override time_t JudgementRadius => MAX_RADIUS;

        public void UserInput(float amount)
        {
        }

        protected override void AdvancePosition(time_t position)
        {
        }

        protected override void ObjectEnteredJudgement(OpenRM.Object obj)
        {
        }

        protected override void ObjectExitedJudgement(OpenRM.Object obj)
        {
        }
    }
}
