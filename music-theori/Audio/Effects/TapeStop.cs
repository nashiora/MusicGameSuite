using System;

namespace theori.Audio.Effects
{
    // TODO(local): SHOULD THE TAPE STOP USE MIX???
    // TODO(local): SHOULD THE TAPE STOP USE MIX???
    // TODO(local): SHOULD THE TAPE STOP USE MIX???
    // TODO(local): SHOULD THE TAPE STOP USE MIX???
    // TODO(local): SHOULD THE TAPE STOP USE MIX???
    public class TapeStop : Dsp
    {
        private double duration = 5;
        private int samplePosition;
        private float floatSamplePosition;
        private float[] sampleBuffer = new float[0];

        public double Duration
        {
            get { return duration; }
            set { SetDuration(value); }
        }

        public TapeStop(int sampleRate)
            : base(sampleRate)
        {
        }

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            if (sampleBuffer.Length == 0) SetDuration(duration);

            int numSamples = count / 2;
            int sampleDuration = sampleBuffer.Length >> 1;

            for(int i = 0; i < numSamples; i++)
            {
                float sampleRate = 1.0f - (float)samplePosition / sampleDuration;
                if(sampleRate <= 0.0f)
                {
                    // Mute
                    buffer[offset + i * 2] = 0.0f;
                    buffer[offset + i * 2 + 1] = 0.0f;
                    continue;
                }

                // Store samples for later
                sampleBuffer[samplePosition * 2] = buffer[offset + i * 2];
                sampleBuffer[samplePosition * 2 + 1] = buffer[offset + i * 2];

                // The sample index into the stored buffer
                int i2 = (int)Math.Floor(floatSamplePosition);
                buffer[offset + i * 2] = sampleBuffer[i2 * 2];
                buffer[offset + i * 2 + 1] = sampleBuffer[i2 * 2 + 1];

                // Increase index
                floatSamplePosition += sampleRate;
                samplePosition++;
            }
        }

        private void SetDuration(double duration)
        {
            this.duration = duration;

            int numSamples = (int)(duration * SampleRate) * 2;
            Array.Resize(ref sampleBuffer, numSamples);
        }
    }

    public sealed class TapeStopEffectDef : EffectDef
    {
        public EffectParamF Duration { get; }
        
        public TapeStopEffectDef(EffectParamF mix, EffectParamF duration)
            : base(EffectType.TapeStop, mix)
        {
            Duration = duration;
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new TapeStop(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is TapeStop ts)
            {
                ts.Duration = Duration.Sample(alpha);
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is TapeStopEffectDef stop)) return false;
            return Type == stop.Type && Mix == stop.Mix && Duration == stop.Duration;
        }

        public override int GetHashCode() => HashCode.For(Type, Mix, Duration);
    }
}
