using System;
using System.Collections.Generic;

using OpenRM;

namespace NeuroSonic.GamePlay.Scoring
{
    public sealed class ButtonJudge : StreamJudge
    {
        public struct Tick
        {
            public OpenRM.Object AssociatedObject;

            public time_t Position;
            public bool IsHold;

            public Tick(OpenRM.Object obj, time_t pos, bool isHold)
            {
                AssociatedObject = obj;
                Position = pos;
                IsHold = isHold;
            }
        }

        public const int TOTAL_MISS_MILLIS = 150;
        public const int TOTAL_NEAR_MILLIS = 100;
        public const int TOTAL_CRIT_MILLIS = 36;
        public const int TOTAL_PERF_MILLIS = 18;

        public const int TOTAL_HOLD_MILLIS = 36;

        private const double MISS_RADIUS = (TOTAL_MISS_MILLIS / 2) / 1000.0;
        private const double NEAR_RADIUS = (TOTAL_NEAR_MILLIS / 2) / 1000.0;
        private const double CRIT_RADIUS = (TOTAL_CRIT_MILLIS / 2) / 1000.0;
        private const double PERF_RADIUS = (TOTAL_PERF_MILLIS / 2) / 1000.0;

        private const double HOLD_RADIUS = (TOTAL_HOLD_MILLIS / 2) / 1000.0;

        private const double MAX_RADIUS = MISS_RADIUS;

        private bool m_userHeld = false;
        private readonly List<Tick> m_ticks = new List<Tick>();

        public event Action<time_t, JudgeResult> OnTickProcessed;

        public ButtonJudge(Chart chart, int streamIndex)
            : base(chart, streamIndex)
        {
        }

        protected override time_t JudgementRadius => MAX_RADIUS;

        public void UserPressed(time_t timeStamp)
        {
            m_userHeld = true;
            if (m_ticks.Count == 0) return;

            var tick = m_ticks[0];
            // Don't ACTUALLY handle holds handled in here
            if (tick.IsHold) return;

            m_ticks.RemoveAt(0);

            // TODO(local): MAKE SURE EVERYTHING IS TRANSFORMED INTO THE CORRECT SPACES
            // I THINK I MISSED SOMETHING WITH "WHERE IS THE CRIT CENTER"

            time_t diff = tick.Position + JudgementOffset - timeStamp;
            time_t absDiff = MathL.Abs(diff.Seconds);

            time_t offsetTime = timeStamp - JudgementOffset;
            if (absDiff <= PERF_RADIUS)
                OnTickProcessed?.Invoke(offsetTime, new JudgeResult(diff, JudgeKind.Perfect));
            else if (absDiff <= CRIT_RADIUS)
                OnTickProcessed?.Invoke(offsetTime, new JudgeResult(diff, JudgeKind.Critical));
            else if (absDiff <= NEAR_RADIUS)
                OnTickProcessed?.Invoke(offsetTime, new JudgeResult(diff, JudgeKind.Near));
            // TODO(local): Is this how we want to handle misses?
            else OnTickProcessed?.Invoke(offsetTime, new JudgeResult(diff, JudgeKind.Miss));
        }

        public void UserReleased(time_t timeStamp)
        {
            m_userHeld = false;
        }

        protected override void AdvancePosition(time_t position)
        {
            // remove old ticks first
            while (m_ticks.Count > 0)
            {
                var tick = m_ticks[0];

                time_t radius = tick.IsHold ? HOLD_RADIUS : MAX_RADIUS;
                if (tick.Position + JudgementOffset < position - radius)
                {
                    m_ticks.RemoveAt(0);
                    OnTickProcessed?.Invoke(position, new JudgeResult(tick.Position + JudgementOffset - position, JudgeKind.Miss));
                }
                else break;
            }

            while (m_ticks.Count > 0)
            {
                var tick = m_ticks[0];
                if (!tick.IsHold) break;

                time_t diff = tick.Position + JudgementOffset - position;
                time_t absDiff = MathL.Abs(diff.Seconds);

                if (absDiff <= HOLD_RADIUS)
                {
                    m_ticks.RemoveAt(0);
                    OnTickProcessed?.Invoke(position - JudgementOffset, new JudgeResult(diff, JudgeKind.Passive));
                }
                else break;
            }
        }

        protected override void ObjectEnteredJudgement(OpenRM.Object obj)
        {
            //Logger.Log($"Button [{ StreamIndex }] entered window @ { obj.AbsolutePosition } / { CurrentPosition }");

            if (obj.IsInstant)
            {
                var chipTick = new Tick(obj, obj.AbsolutePosition, false);
                m_ticks.Add(chipTick);
            }
        }

        protected override void ObjectExitedJudgement(OpenRM.Object obj)
        {
            //Logger.Log($"Button [{ StreamIndex }] left window @ { obj.AbsoluteEndPosition } / { CurrentPosition }");
        }
    }
}
