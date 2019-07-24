using System;

namespace theori.Audio.Effects
{
    public sealed class BitCrusher : Dsp
    {
        private double samplePosition;
        private double sampleScale;

        private float sampleLeft;
        private float sampleRight;

        public double Reduction = 4 / 44100.0f;

        public BitCrusher(int sampleRate)
            : base(sampleRate)
        {
            sampleScale = sampleRate / 44100.0f;
        }

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;

            for(int i = 0; i < numSamples; i++)
            {
                samplePosition += 1.0 / SampleRate;
                if(samplePosition > Reduction * sampleScale)
                {
                    sampleLeft = buffer[offset + i * 2];
                    sampleRight = buffer[offset + i * 2 + 1];
                    samplePosition -= Reduction * sampleScale;
                }

                buffer[offset + i * 2] = MathL.Lerp(buffer[offset + i * 2], sampleLeft, Mix);
                buffer[offset + i * 2 + 1] = MathL.Lerp(buffer[offset + i * 2 + 1], sampleRight, Mix);
            }
        }
    }
}
