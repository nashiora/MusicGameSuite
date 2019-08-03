using System;

namespace theori.Audio.Effects
{
    public class Phaser : Dsp
    {
        private const int NumBands = 8;

        private float feedback = 0.15f;
        private double time;
        private APF[] allPassFilters = new APF[NumBands * 2]; // 6 bands - Stereo
        private float[] feedbackBuffer = new float[2];
        private float maxmimumFrequency = 6000.0f;
        private float minimumFrequency = 1000.0f;
        private float frequencyDelta;

        public Phaser(int sampleRate)
            : base(sampleRate)
        {
            CalculateFrequencyDelta();
        }

        public override void Reset()
        {
            time = 0.0f;
            feedbackBuffer.Fill(0.0f);
            allPassFilters.Fill(new APF());
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

        public double Duration { get; set; } = 2.0;

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;

            float sampleRateFloat = SampleRate;
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
                    float filtered = buffer[i * 2 + c] + feedbackBuffer[c] * feedback;
                    for (int b = NumBands - 1; b >= 0; b--)
                        filtered = allPassFilters[b + filterOffset].Update(filtered);

                    // Store filter feedback
                    feedbackBuffer[c] = filtered;

                    // Final sample
                    buffer[offset + i * 2 + c] = buffer[offset + i * 2 + c] + filtered * Mix;
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
                y = a1 * (y + input) - x;
                x = input;
                //float y = input * -a1 + za;
                //za = y * a1 + input;
                return y;
            }

            float y, x;

            public float a1;
            public float za;
        };
    }
}
