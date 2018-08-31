using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenRM;
using OpenRM.Voltex;

namespace theori.Game
{
    public sealed class HighwayControl
    {
        private const float LASER_BASE_STRENGTH = 12;

        public static LaserParams DefaultLaserParams { get; } = new LaserParams()
        {
            Function = LaserFunction.Source,
        };

        class Timed<T>
            where T : struct
        {
            public readonly time_t StartTime;
            public readonly T Params;

            public Timed(time_t startTime, T p)
            {
                StartTime = startTime;
                Params = p;
            }
        }

        #region Private Data
        
        private LaserParams m_leftLaserParams = DefaultLaserParams;
        private LaserParams m_rightLaserParams = DefaultLaserParams;
        
        private float m_leftLaserInput, m_rightLaserInput;
        private float m_combinedLaserOutput, m_targetCombinedLaserOutput;
        
        private float m_zoom, m_pitch, m_offset, m_roll;
        private float m_effectRoll, m_effectOffset;

        private LinearDirection m_selectedLaser = LinearDirection.None;
        private LaserApplication m_laserApplication = LaserApplication.Additive;
        private Damping m_laserDamping = Damping.Slow;
        
        private Timed<SpinParams> m_spin;
        private Timed<SwingParams> m_swing;
        private Timed<WobbleParams> m_wobble;

        #endregion

        #region Programmable Control Interface

        public LaserParams LeftLaserParams  { set => m_leftLaserParams  = value; }
        public LaserParams RightLaserParams { set => m_rightLaserParams = value; }
        
        public float LeftLaserInput  { set { m_leftLaserInput = value; } }
        public float RightLaserInput { set { m_rightLaserInput = value; } }
        
        public float LaserRoll { get { return m_combinedLaserOutput; } }

        public float Zoom { set { m_zoom = value; } }
        public float Pitch { set { m_pitch = value; } }
        public float Offset { set { m_offset = value; } }
        public float Roll { get => m_roll; set => m_roll = value; }

        public float EffectOffset { get { return m_effectOffset; } }
        public float EffectRoll { get { return m_effectRoll; } }

        public LaserApplication LaserApplication { set => m_laserApplication = value; }
        public Damping LaserDamping { set => m_laserDamping = value; }

        /// <summary>
        /// Applies a full spin (360 rotation with recovery animation)
        ///  to this highway using the given associated parameters.
        /// </summary>
        public void ApplySpin(SpinParams p)
        {
            m_spin = new Timed<SpinParams>(Time.Total, p);
        }

        /// <summary>
        /// Applies a back-and-forth swing to this highway
        ///  using the given associated parameters.
        /// </summary>
        public void ApplySwing(SwingParams p)
        {
            m_swing = new Timed<SwingParams>(Time.Total, p);
        }
        
        /// <summary>
        /// Applies a horizontal "wobble" to this highway
        ///  using the given associated parameters.
        /// </summary>
        public void ApplyWobble(WobbleParams p)
        {
            m_wobble = new Timed<WobbleParams>(Time.Total, p);
        }

        #endregion

        public HighwayControl()
        {
        }

        public void ApplyToView(HighwayView view)
        {
            view.TargetLaserRoll = m_combinedLaserOutput;
            view.TargetZoom = m_zoom;
            view.TargetPitch = m_pitch;
            view.TargetOffset = m_offset + m_effectOffset;
            view.TargetBaseRoll = m_roll + m_effectRoll;
        }

        public void Update()
        {
            float leftLaser = -ProcessLaserInput(ref m_leftLaserInput, m_leftLaserParams);
            float rightLaser = ProcessLaserInput(ref m_rightLaserInput, m_rightLaserParams);
            
            var appFlag = m_laserApplication & LaserApplication.FlagMask;
            var appValue = m_laserApplication & LaserApplication.ApplicationMask;

            float laserOutput = 0;

            switch (appValue)
            {
                case LaserApplication.Zero: m_selectedLaser = LinearDirection.None; break;
                case LaserApplication.Additive: laserOutput = leftLaser + rightLaser; m_selectedLaser = LinearDirection.None; break;

                case LaserApplication.Left: laserOutput = leftLaser; m_selectedLaser = LinearDirection.Left; break;
                case LaserApplication.Right: laserOutput = rightLaser; m_selectedLaser = LinearDirection.Right; break;

                case LaserApplication.Initial:
                {
                    if (m_selectedLaser == LinearDirection.None)
                    {
                        if (leftLaser == 0)
                        {
                            if (rightLaser != 0)
                                m_selectedLaser = LinearDirection.Right;
                        }
                        else if (rightLaser == 0)
                            m_selectedLaser = LinearDirection.Left;
                    }
                    
                    // apply the selected laser, if one has been selected
                    if (m_selectedLaser == LinearDirection.Left)
                        laserOutput = leftLaser;
                    else if (m_selectedLaser == LinearDirection.Right)
                        laserOutput = rightLaser;
                } break;
            }

            switch (appFlag)
            {
                case LaserApplication.KeepMax:
                {
                    if (m_combinedLaserOutput < 0)
                        laserOutput = MathL.Min(laserOutput, m_combinedLaserOutput);
                    else if (m_combinedLaserOutput > 0)
                        laserOutput = MathL.Max(laserOutput, m_combinedLaserOutput);
                } break;

                case LaserApplication.KeepMin:
                {
                    if (m_combinedLaserOutput < 0)
                        laserOutput = MathL.Max(laserOutput, m_combinedLaserOutput);
                    else if (m_combinedLaserOutput > 0)
                        laserOutput = MathL.Min(laserOutput, m_combinedLaserOutput);
                } break;
            }

            m_targetCombinedLaserOutput = laserOutput;
            switch (m_laserDamping)
            {
                case Damping.Fast: LerpTo(ref m_combinedLaserOutput, m_targetCombinedLaserOutput, 1.50f, 25); break;
                case Damping.Slow: LerpTo(ref m_combinedLaserOutput, m_targetCombinedLaserOutput, 0.15f, 10); break;
                case Damping.Off:
                {
                    const int SPEED = 60;
                    if (m_targetCombinedLaserOutput < m_combinedLaserOutput)
                        m_combinedLaserOutput = Math.Max(m_targetCombinedLaserOutput, m_combinedLaserOutput - Time.Delta * SPEED);
                    else m_combinedLaserOutput = Math.Min(m_targetCombinedLaserOutput, m_combinedLaserOutput + Time.Delta * SPEED);
                } break;
            }
            
            float spinRoll = 0;
            float swingRoll = 0;
            float wobbleOffset = 0;

            if (m_spin != null)
            {
                if (m_spin.StartTime + m_spin.Params.Duration < Time.Total)
                    m_spin = null;
                else
                {
                    float time = (float)((Time.Total - m_spin.StartTime) / m_spin.Params.Duration);
                    //Trace.WriteLine($"SPIN CONTROL: from { m_spin.StartTime } for { m_spin.Params.Duration }, { time }");
                    float dir = (int)m_spin.Params.Direction;

	                const float TSPIN = 0.75f / 2.0f;
	                const float TRECOV = 0.75f / 2.0f;

	                //float bgAngle = MathL.Clamp(time * 4.0f, 0.0f, 2.0f) * dir;
	                if (time <= TSPIN)
                    {
                        Trace.WriteLine($"SPIN CONTROL: from { m_spin.StartTime } for { m_spin.Params.Duration }, { time }");
		                spinRoll = -dir * (TSPIN - time) / TSPIN;
                    }
	                else
	                {
		                if (time < TSPIN + TRECOV)
			                spinRoll = DampedSin((time - TSPIN) / TRECOV, 30f / 360, 0.5f, 0) * dir;
		                else spinRoll = 0.0f;
	                }
                }
            }

            if (m_swing != null)
            {
                if (m_swing.StartTime + m_swing.Params.Duration < Time.Total)
                    m_swing = null;
                else
                {
                    float time = (float)((Time.Total - m_swing.StartTime) / m_swing.Params.Duration);
                    float dir = (int)m_swing.Params.Direction;

                    #if false
                    // dividing the amplitude by 0.5625 makes the first crest of the sin
                    //  wave reach exactly that amplitude, as its damped quadradically.
                    // A frequency of 1 leaves 1 crest and 1 trough of the wave,
                    //  so at time 0.25 the first crest is reached.
                    // The damping equation is applied quadratically in terms of
                    //  1 - time, which is 0.75.
                    // 0.75 ^ 2 = 0.5625.
                    // At time 0.25 the amplitude of the wave is exactly what
                    //  the setting wants it to be.
			        swingRoll = DampedSin(time, (m_swing.Params.Amplitude / 0.5625f) / 360, 1, 0) * dir;
                    #else
			        swingRoll = DampedSin(time, (m_swing.Params.Amplitude / 0.75f) / 360, 1, 0) * dir;
                    #endif
                }
            }

            if (m_wobble != null)
            {
                if (m_wobble.StartTime + m_wobble.Params.Duration < Time.Total)
                    m_wobble = null;
                else
                {
                    float time = (float)((Time.Total - m_wobble.StartTime) / m_wobble.Params.Duration);
                    float dir = (int)m_wobble.Params.Direction;

                    float decay = 0;
                    switch (m_wobble.Params.Decay)
                    {
                        case Decay.Off: decay = 1; break;
                        case Decay.OnSlow: decay = 0.5f; break;
                        case Decay.On: decay = 0; break;
                    }

			        wobbleOffset = DampedSin(time, m_wobble.Params.Amplitude * 0.5f,
				                   m_wobble.Params.Frequency / 2.0f, decay) * dir;
                }
            }

            m_effectRoll = spinRoll + swingRoll;
            m_effectOffset = wobbleOffset;

            float DampedSin(float t, float amplitude, float frequency, float decayTo)
            {
                //float decay = MathL.Lerp(1, decayTo, 1 - (1 - t) * (1 - t));
                float decay = MathL.Lerp(1, decayTo, t);
                return amplitude * decay * MathL.Sin(frequency * 2 * t * MathL.Pi_f);
            }

            float ProcessLaserInput(ref float value, LaserParams p)
            {
                float output = value;

                switch (p.Function & LaserFunction.FunctionMask)
                {
                    case LaserFunction.Zero: return 0;
                    case LaserFunction.Source: break;
                    case LaserFunction.NegativeSource: output = -output; break;
                    case LaserFunction.OneMinusSource: output = 1 - output; break;
                }
            
                switch (p.Function & LaserFunction.FlagMask)
                {
                    case LaserFunction.Normal: break;
                    case LaserFunction.Smaller: output *= 0.5f; break;
                    case LaserFunction.Bigger: output *= 1.5f; break;
                    case LaserFunction.Biggest: output *= 2.0f; break;
                }

                return output * LASER_BASE_STRENGTH;
            }

            void LerpTo(ref float value, float target, float max, float speed)
            {
                float diff = MathL.Abs(target - value);
                float change = diff * Time.Delta * speed;
                change = MathL.Min(max, change);

                if (target < value)
                    value = MathL.Max(value - change, target);
                else value = MathL.Min(value + change, target);
            }
        }
    }
}
