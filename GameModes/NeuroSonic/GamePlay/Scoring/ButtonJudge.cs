using System;
using System.Collections.Generic;

using theori;
using theori.Charting;

namespace NeuroSonic.GamePlay.Scoring
{
    public sealed class ButtonJudge : StreamJudge
    {
        public struct Tick
        {
            public ChartObject AssociatedObject;

            public time_t Position;
            public bool IsHold;

            public bool IsAutoTick;

            public Tick(ChartObject obj, time_t pos, bool isHold, bool isAutoTick = false)
            {
                AssociatedObject = obj;
                Position = pos;
                IsHold = isHold;

                IsAutoTick = isAutoTick;
            }
        }

        public const int TOTAL_MISS_MILLIS = 150 * 2;
        public const int TOTAL_NEAR_MILLIS = 100 * 2;
        public const int TOTAL_CRIT_MILLIS = 50 * 2;
        public const int TOTAL_PERF_MILLIS = 25 * 2;

        public const int TOTAL_HOLD_MILLIS = 12 * 2;

        private const double MISS_RADIUS = (TOTAL_MISS_MILLIS / 2) / 1000.0;
        private const double NEAR_RADIUS = (TOTAL_NEAR_MILLIS / 2) / 1000.0;
        private const double CRIT_RADIUS = (TOTAL_CRIT_MILLIS / 2) / 1000.0;
        private const double PERF_RADIUS = (TOTAL_PERF_MILLIS / 2) / 1000.0;

        private const double HOLD_RADIUS = (TOTAL_HOLD_MILLIS / 2) / 1000.0;

        private const double MAX_RADIUS = MISS_RADIUS;

        private bool m_userHeld = false;
        private time_t m_userWhen = 0.0;
        private ChartObject m_lastPressedObject;

        private readonly List<Tick> m_ticks = new List<Tick>();

        public event Action<time_t, ChartObject> OnChipPressed;

        public event Action<time_t, ChartObject> OnHoldPressed;
        public event Action<time_t, ChartObject> OnHoldReleased;

        public event Action<ChartObject, time_t, JudgeResult> OnTickProcessed;

        private readonly tick_t m_holdTickStep;
        private readonly tick_t m_holdTickMargin;

        public ButtonJudge(Chart chart, int streamIndex)
            : base(chart, streamIndex)
        {
            m_holdTickStep = (Chart.MaxBpm >= 255 ? 2.0 : 1.0) / (4 * 2);
            m_holdTickMargin = 2 * m_holdTickStep;
        }

        protected override time_t JudgementRadius => MAX_RADIUS;

        public JudgeResult? UserPressed(time_t timeStamp)
        {
            if (AutoPlay) return null;

            m_userHeld = true;
            m_userWhen = timeStamp;

            if (m_ticks.Count == 0) return null;

            var tick = m_ticks[0];
            // Don't ACTUALLY handle holds handled in here
            if (tick.IsHold)
            {
                OnHoldPressed?.Invoke(timeStamp, tick.AssociatedObject);
                m_lastPressedObject = tick.AssociatedObject;
                return null;
            }
            else OnChipPressed?.Invoke(timeStamp, tick.AssociatedObject);

            m_ticks.RemoveAt(0);

            time_t diff = tick.Position + JudgementOffset - timeStamp;
            time_t absDiff = MathL.Abs(diff.Seconds);

            time_t offsetTime = timeStamp - JudgementOffset;

            JudgeResult result;
            if (absDiff <= PERF_RADIUS)
                result = new JudgeResult(diff, JudgeKind.Perfect);
            else if (absDiff <= CRIT_RADIUS)
                result = new JudgeResult(diff, JudgeKind.Critical);
            else if (absDiff <= NEAR_RADIUS)
                result = new JudgeResult(diff, JudgeKind.Near);
            // TODO(local): Is this how we want to handle misses?
            else result = new JudgeResult(diff, JudgeKind.Bad);

            OnTickProcessed?.Invoke(tick.AssociatedObject, offsetTime, result);
            return result;
        }

        public void UserReleased(time_t timeStamp)
        {
            m_userHeld = false;

            if (m_ticks.Count > 0 && m_ticks[0].IsHold && m_lastPressedObject == m_ticks[0].AssociatedObject)
            {
                OnHoldReleased?.Invoke(timeStamp, m_ticks[0].AssociatedObject);
            }
        }

        protected override void AdvancePosition(time_t position)
        {
            // remove old ticks first
            while (!AutoPlay && m_ticks.Count > 0)
            {
                var tick = m_ticks[0];

                time_t radius = tick.IsHold ? HOLD_RADIUS : MAX_RADIUS;
                if (tick.Position + JudgementOffset < position - radius)
                {
                    m_ticks.RemoveAt(0);
                    OnTickProcessed?.Invoke(tick.AssociatedObject, position, new JudgeResult(tick.Position + JudgementOffset - position, JudgeKind.Miss));
                }
                else break;
            }

            while (m_ticks.Count > 0)
            {
                var tick = m_ticks[0];
                if (AutoPlay && position >= tick.Position)
                {
                    m_ticks.RemoveAt(0);
                    if (tick.IsAutoTick)
                    {
                        if (tick.Position == tick.AssociatedObject.AbsolutePosition)
                            OnHoldPressed?.Invoke(position, tick.AssociatedObject);
                        //else OnHoldReleased?.Invoke(position, tick.AssociatedObject);
                    }
                    else if (tick.IsHold)
                    {
                        OnTickProcessed?.Invoke(tick.AssociatedObject, position - JudgementOffset, new JudgeResult(0, JudgeKind.Passive));
                    }
                    else
                    {
                        OnChipPressed?.Invoke(position, tick.AssociatedObject);
                        OnTickProcessed?.Invoke(tick.AssociatedObject, tick.Position, new JudgeResult(0, JudgeKind.Perfect));
                    }
                }
                else // ===== NO AUTO PLAY =====
                {
                    if (!tick.IsHold) break;

                    time_t check = tick.AssociatedObject.AbsolutePosition + JudgementOffset - m_userWhen;
                    if (check > MISS_RADIUS) break;

                    time_t diff = tick.Position + JudgementOffset - position;
                    time_t absDiff = MathL.Abs(diff.Seconds);

                    if (m_userHeld && diff > 0 && absDiff <= HOLD_RADIUS)
                    {
                        m_ticks.RemoveAt(0);
                        OnTickProcessed?.Invoke(tick.AssociatedObject, position - JudgementOffset, new JudgeResult(diff, JudgeKind.Passive));
                    }
                    else break;
                }
            }
        }

        public override int CalculateNumScorableTicks()
        {
            int tickCount = 0;
            foreach (var obj in Chart[StreamIndex])
            {
                if (obj.IsInstant)
                    tickCount++;
                else tickCount += MathL.Max(1, MathL.FloorToInt((double)(obj.Duration - m_holdTickMargin) / (double)m_holdTickStep));
            }
            return tickCount;
        }

        protected override void ObjectEnteredJudgement(ChartObject obj)
        {
            if (AutoPlay && !obj.IsInstant)
                m_ticks.Add(new Tick(obj, obj.AbsolutePosition, true, true));

            if (obj.IsInstant)
            {
                var chipTick = new Tick(obj, obj.AbsolutePosition, false);
                m_ticks.Add(chipTick);
            }
            else
            {

                int numTicks = MathL.FloorToInt((double)(obj.Duration - m_holdTickMargin) / (double)m_holdTickStep);

                if (numTicks == 0)
                    m_ticks.Add(new Tick(obj, obj.AbsolutePosition + obj.AbsoluteDuration / 2, true));
                else
                {
                    tick_t pos = obj.Position + m_holdTickMargin / 2;
                    for (int i = 0; i < numTicks; i++)
                    {
                        time_t timeAtTick = Chart.CalcTimeFromTick(pos + i * m_holdTickStep);
                        m_ticks.Add(new Tick(obj, timeAtTick, true));
                    }
                }
            }

            if (AutoPlay && !obj.IsInstant)
                m_ticks.Add(new Tick(obj, obj.AbsoluteEndPosition, true, true));
        }

        protected override void ObjectExitedJudgement(ChartObject obj)
        {
            if (m_lastPressedObject == obj)
                m_lastPressedObject = null;
        }
    }
}
