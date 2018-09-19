using System;

namespace OpenRM.Audio.Effects
{
    public class Phaser : Dsp
    {
        private const int NumBands = 6;

        private float feedback = 0.35f;
        private double time;
        private APF[] allPassFilters = new APF[NumBands * 2]; // 6 bands - Stereo
        private float[] feedbackBuffer = new float[2];
        private float maxmimumFrequency = 20000.0f;
        private float minimumFrequency = 12000.0f;
        private float frequencyDelta;

        public Phaser(int sampleRate)
            : base(sampleRate)
        {
        }

        public float MinimumFrequency
        {
            get { return minimumFrequency; }
            set
            {
                minimumFrequency = value;
                CalculateFrequencyDelta();
            }
        }

        public float MaxmimumFrequency
        {
            get { return maxmimumFrequency; }
            set
            {
                maxmimumFrequency = value;
                CalculateFrequencyDelta();
            }
        }

        public float Feedback
        {
            get { return feedback; }
            set { feedback = MathL.Clamp(value, 0.0f, 1.0f); }
        }

        public double Duration { get; set; } = 1.0;

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;

            float sampleRateFloat = (float)SampleRate;
            double sampleStep = 1.0 / SampleRate;

            for(int i = 0; i < numSamples; i++)
            {
                float f = (float)((time % Duration) / Duration) * MathL.TwoPi;

                //calculate and update phaser sweep lfo...
                float d = minimumFrequency + frequencyDelta * (((float)Math.Sin(f) + 1.0f) / 2.0f);
                d /= sampleRateFloat;

                //calculate output per channel
                for(int c = 0; c < 2; c++)
                {
                    int filterOffset = c * NumBands;

                    //update filter coeffs
                    float a1 = (1.0f - d) / (1.0f + d);
                    for(int j = 0; j < NumBands; j++)
                        allPassFilters[j + filterOffset].a1 = a1;

                    // Calculate ouput from filters chained together
                    // Merry christmas!
                    float filtered = allPassFilters[0 + filterOffset].Update(
                        allPassFilters[1 + filterOffset].Update(
                            allPassFilters[2 + filterOffset].Update(
                                allPassFilters[3 + filterOffset].Update(
                                    allPassFilters[4 + filterOffset].Update(
                                        allPassFilters[5 + filterOffset].Update(buffer[i * 2 + c] + feedbackBuffer[c] * feedback))))));

                    // Store filter feedback
                    feedbackBuffer[c] = filtered;

                    // Final sample
                    buffer[offset + i * 2 + c] = buffer[offset + i * 2 + c] + filtered;
                }

                time += sampleStep;
            }
        }

        private void CalculateFrequencyDelta()
        {
            frequencyDelta = maxmimumFrequency - minimumFrequency;
        }

        public struct APF
        {
            public float Update(float input)
            {
                float y = input * -a1 + za;
                za = y * a1 + input;
                return y;
            }

            public float a1;
            public float za;
        };
    }

    public sealed class PhaserEffectDef : EffectDef
    {
        public PhaserEffectDef(EffectParamF mix)
            : base(EffectType.Phaser, mix)
        {
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new Phaser(sampleRate);

        public override void ApplyToDsp(Dsp effect, float alpha = 0)
        {
            base.ApplyToDsp(effect, alpha);
            if (effect is Phaser p)
            {
            }
        }
    }
}
