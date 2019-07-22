using System;

namespace theori.Audio.Effects
{
    public sealed class Retrigger : Dsp, IMixable
    {
        /// <summary>
        /// Duration of the part to retrigger (in seconds)
        /// </summary>
        public double Duration = 0.1;

        /// <summary>
        /// How many times to loop before restarting the retriggered part
        /// </summary>
        public int LoopCount = 8;

        /// <summary>
        /// Amount(0,1) of time to mute before playing again, 1 is fully gated
        /// </summary>
        public float Gating;

        private float[] retriggerBuffer = new float[0];
        private int currentSample = 0;
        private int currentLoop = 0;

        public Retrigger(int sampleRate)
            : base(sampleRate)
        {
        }

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;

            double sll = SampleRate * Duration;
            int sampleDuration = (int)(SampleRate * Duration);
            int sampleGatingLength = (int)(sll * Gating);

            if(retriggerBuffer.Length < (sampleDuration*2))
                Array.Resize(ref retriggerBuffer, sampleDuration * 2);

            for(int i = 0; i < numSamples; i++)
            {
                if(currentLoop == 0)
                {
                    // Store samples for later
                    if(currentSample > sampleGatingLength) // Additional gating
                    {
                        retriggerBuffer[currentSample*2] = (0.0f);
                        retriggerBuffer[currentSample*2+1] = (0.0f);
                    }
                    else
                    {
                        retriggerBuffer[currentSample*2] = buffer[i * 2];
                        retriggerBuffer[currentSample*2+1] = buffer[i * 2 + 1];
                    }
                }

                // Sample from buffer
                buffer[offset + i * 2] = MathL.Lerp(buffer[offset + i * 2], retriggerBuffer[currentSample * 2], Mix);
                buffer[offset + i * 2 + 1] = MathL.Lerp(buffer[offset + i * 2 + 1], retriggerBuffer[currentSample * 2 + 1], Mix);
		
                // Increase index
                currentSample++;
                if(currentSample >= sampleDuration)
                {
                    currentSample -= sampleDuration;
                    currentLoop++;
                    if(LoopCount != 0 && currentLoop >= LoopCount)
                    {
                        // Reset
                        currentLoop = 0;
                        currentSample = 0;
                    }
                }
            }
        }
    }

    public sealed class RetriggerEffectDef : EffectDef
    {
        public EffectParamF GateDuration;
        public EffectParamF Gating;

        public RetriggerEffectDef() : base(EffectType.Retrigger) { }
        
        public RetriggerEffectDef(EffectParamF mix, EffectParamF gating, EffectParamF gateDuration)
            : base(EffectType.Retrigger, mix)
        {
            GateDuration = gateDuration;
            Gating = gating;
        }
        
        public override Dsp CreateEffectDsp(int sampleRate) => new Retrigger(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is Retrigger retrigger)
            {
                retrigger.Mix = Mix.Sample(alpha);
                retrigger.Gating = Gating.Sample(alpha);
                retrigger.Duration = GateDuration.Sample(alpha) * qnDur.Seconds * 4;
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is RetriggerEffectDef rt)) return false;
            return Type == rt.Type && Mix == rt.Mix && GateDuration == rt.GateDuration && Gating == rt.Gating;
        }

        public override int GetHashCode() => HashCode.For(Type, Mix, GateDuration, Gating);
    }
}
