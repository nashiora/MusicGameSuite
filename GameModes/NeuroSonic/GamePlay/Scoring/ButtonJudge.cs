using System;

using OpenRM;

namespace NeuroSonic.GamePlay.Scoring
{
    public sealed class ButtonJudge : StreamJudge
    {
        public const int TOTAL_CRIT_MILLIS = 36;

        private const double CRIT_RADIUS = TOTAL_CRIT_MILLIS / 1000.0;

        public ButtonJudge(Chart chart, int streamIndex)
            : base(chart, streamIndex)
        {
        }

        protected override time_t JudgementRadius => CRIT_RADIUS;

        protected override void ObjectEnteredJudgement(OpenRM.Object obj)
        {
            Logger.Log($"Button [{ StreamIndex }] entered window @ { obj.AbsolutePosition } / { CurrentPosition }");
        }

        protected override void ObjectExitedJudgement(OpenRM.Object obj)
        {
            Logger.Log($"Button [{ StreamIndex }] left window @ { obj.AbsolutePosition } / { CurrentPosition }");
        }
    }
}
