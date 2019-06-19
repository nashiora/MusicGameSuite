using System;
using System.Collections.Generic;

using OpenRM;
using OpenRM.Voltex;

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

        enum InputState
        {
            /// <summary>
            /// Valid inputs have been inputed, the system will expect more valid inputs.
            /// 
            /// If no inputs are given for a short while, the UnlockedActive input state
            ///  is triggered until more valid inputs are given.
            /// </summary>
            LockedActive,

            /// <summary>
            /// No inputs have been entered, but the laser is not incactive yet.
            /// 
            /// If valid inputs are given to the current motion state, we go back to
            ///  the LockedActive input state.
            /// If the cursor falls too far outside of the laser or invalid inputs are
            ///  given then the Inactive input state is triggered.
            /// </summary>
            UnlockedActive,

            /// <summary>
            /// Invalid inputs have been inputed, so we're waiting for more valid ones.
            /// 
            /// If the cursor fals back inside the laser (and subsequent valid inputs are
            ///  given in most cases) then the LockedActive input state is triggered again.
            /// </summary>
            Inactive,
        }

        enum MotionState
        {
            /// <summary>
            /// There is no laser to follow.
            /// 
            /// Any input is valid and will do nothing, no matter the input state.
            /// </summary>
            Idle,

            /// <summary>
            /// The state of NOT currently beign in a laser, but
            ///  begin ready for it.
            /// Cursor movement is not yet active but will reset to the
            ///  start value of the upcoming laser.
            ///  
            /// Any input is valid if either of the *Active input states are in use
            ///  and will do nothing, and any input is invalid if the Inactive
            ///  input state is in use.
            /// </summary>
            AnticipateBegin,

            /// <summary>
            /// During a laser segment; all segments will start with this state.
            /// At the end of a segment where the direction is changing, the 
            ///  AnticipateDirectionSwitch state is used (after this state has been
            ///  used at least once for valid input)
            ///  
            /// In the LockedActive input state, only inputs in the correct
            ///  direction are considered valid; inputs in the incorrect direction
            ///  will trigger the UnlockedActive input state, and no inputs at all
            ///  will do the same after a short while.
            /// In the Inactive input state any input is invalid until the usual
            ///  re-activation criteria are met.
            /// </summary>
            SingleDirection,
            /// <summary>
            /// The state of a segment with the same start and end alpha values.
            /// 
            /// In the UnlockedActive input state, only input values in the direction
            ///  towards the target cursor values are valid and set the input state
            ///  to LockedActive; invalid inputs set it to Inactive immediately.
            /// The LockedActive input state accepts all inputs and does nothing.
            /// In the Inactive input state any input is invalid until the
            ///  usual re-activation criteria are met.
            /// </summary>
            HoldPosition,

            /// <summary>
            /// After a valid input for SingleDirection, when the direction
            ///  changes this state is used.
            /// Both directions will be accepted with different outcomes:
            /// If the previous direction is inputed, that's fine and this state is
            ///  kept as is.
            /// If the new direction is inputed, the state switches to the next (which
            ///  will most likely only ever be SingleDirection)
            ///  
            /// In the LockedActive input state, an input in the new direction immediately
            ///  switches to the next state (should always be SingleDirection next)
            /// </summary>
            AnticipateDirectionSwitch,
            /// <summary>
            /// The state of a slam being near.
            /// No matter the previous state
            /// </summary>
            AnticipateSlam,
        }

        public const int TOTAL_LASER_MILLIS = 72 * 2;

        private const double LASER_RADIUS = (TOTAL_LASER_MILLIS / 2) / 1000.0;

        private const double MAX_RADIUS = LASER_RADIUS;

        private const float ALLOWED_INACCURACY = 0.05f;

        public float CursorPosition { get; private set; } = 0.0f;
        public bool Active => MathL.Abs(CursorPosition - m_targetCursorPosition) <= ALLOWED_INACCURACY;

        private bool m_userInputed = false;
        private time_t m_userWhen = 0.0;
        private time_t m_assistWhen;

        private bool m_lockedOn = true;

        private readonly List<Tick> m_ticks = new List<Tick>();

        private LinearDirection m_targetDirection = LinearDirection.None;
        private float m_targetCursorPosition = 0.0f;
        private AnalogObject m_currentObject;

        public event Action<time_t, OpenRM.Object> OnSlamHit;

        public event Action<time_t, OpenRM.Object> OnLaserActivated;
        public event Action<time_t, OpenRM.Object> OnLaserDeactivated;

        public event Action<OpenRM.Object, time_t, JudgeResult> OnTickProcessed;

        public LaserJudge(Chart chart, int streamIndex)
            : base(chart, streamIndex)
        {
        }

        protected override time_t JudgementRadius => MAX_RADIUS;

        public void UserInput(float amount, time_t timeStamp)
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
