using System;
using System.Diagnostics;

namespace OpenRM.Audio.Effects
{
    public sealed class BiQuadFilter : Dsp
    {
        private const uint order = 2;

        private float b0 = 1;
        private float b1 = 0;
        private float b2 = 0;
        private float a0 = 1;
        private float a1 = 0;
        private float a2 = 0;
        
        private readonly float[,] zb = new float[2, order];
        private readonly float[,] za = new float[2, order];

        public BiQuadFilter(int sampleRate)
            : base(sampleRate)
        {
        }

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;

            for(int i = 0; i < numSamples; i++)
            {
                for (uint c = 0; c < 2; c++)
                {
                    float src = buffer[offset + i * 2 + c];

                    float filtered =
				        (b0 / a0) * src +
				        (b1 / a0) * zb[c, 0] +
				        (b2 / a0) * zb[c, 1] -
				        (a1 / a0) * za[c, 0] -
				        (a2 / a0) * za[c, 1];

			        // Shift delay buffers
			        zb[c, 1] = zb[c, 0];
			        zb[c, 0] = src;

			        // Feedback the calculated value into the IIR delay buffers
			        za[c, 1] = za[c, 0];
			        za[c, 0] = filtered;

                    //sample = filtered;
                    buffer[offset + i * 2 + c] = MathL.Lerp(src, filtered, Mix);
                }
            }
        }

        public void SetLowPass(float q, float freq) => SetLowPass(q, freq, SampleRate);
        public void SetLowPass(float q, float freq, int sampleRate)
        {
	        // Limit q
	        q = Math.Max(q, 0.01f);

	        // Sampling frequency
	        double w0 = (2 * MathL.Pi * freq) / sampleRate;
	        double cw0 = Math.Cos(w0);
	        float alpha = (float)(Math.Sin(w0) / (2 * q));

	        b0 = (float)((1 - cw0) / 2);
	        b1 = (float)(1 - cw0);
	        b2 = (float)((1 - cw0) / 2);
	        a0 = 1 + alpha;
	        a1 = (float)(-2 * cw0);
	        a2 = 1 - alpha;
        }

        public void SetHighPass(float q, float freq) => SetHighPass(q, freq, SampleRate);
        public void SetHighPass(float q, float freq, int sampleRate)
        {
            // Limit q
	        q = Math.Max(q, 0.01f);

            Debug.Assert(freq < sampleRate, "freq !< sampleRate");
            double w0 = (2 * MathL.Pi * freq) / sampleRate;
            double cw0 = Math.Cos(w0);
            float alpha = (float)(Math.Sin(w0) / (2 * q));

            b0 = (float)((1 + cw0) / 2);
            b1 = (float)-(1 + cw0);
            b2 = (float)((1 + cw0) / 2);
            a0 = 1 + alpha;
            a1 = (float)(-2 * cw0);
            a2 = 1 - alpha;
        }

        public void SetPeaking(float q, float freq, float gain) => SetPeaking(q, freq, gain, SampleRate);
        public void SetPeaking(float q, float freq, float gain, int sampleRate)
        {
	        // Limit q
	        q = Math.Max(q, 0.01f);

	        double w0 = (2 * MathL.Pi * freq) / sampleRate;
	        double cw0 = Math.Cos(w0);
	        float alpha = (float)(Math.Sin(w0) / (2 * q));
	        double A = Math.Pow(10, (gain / 40));

	        b0 = 1 + (float)(alpha * A);
	        b1 = -2 * (float)cw0;
	        b2 = 1 - (float)(alpha*A);
	        a0 = 1 + (float)(alpha / A);
	        a1 = -2 * (float)cw0;
	        a2 = 1 - (float)(alpha / A);
        }
    }

    public sealed class BiQuadFilterEffectDef : EffectDef
    {
        public EffectParamF Q { get; }

        public EffectParamF Gain { get; }
        public EffectParamF Freq { get; }
        
        public BiQuadFilterEffectDef(EffectType type, EffectParamF mix,
            EffectParamF q, EffectParamF gain, EffectParamF freq)
            : base(type, mix)
        {
            Q = q;
            Gain = gain;
            Freq = freq;
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new BiQuadFilter(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is BiQuadFilter filter)
            {
                switch (Type)
                {
                    case EffectType.PeakingFilter:
                        filter.SetPeaking(Q.Sample(alpha), Freq.Sample(alpha), Gain.Sample(alpha));
                        break;
                        
                    case EffectType.LowPassFilter:
                        filter.SetLowPass(Q.Sample(alpha) * Mix.Sample(alpha) + 0.1f, Freq.Sample(alpha));
                        break;
                        
                    case EffectType.HighPassFilter:
                        filter.SetHighPass(Q.Sample(alpha) * Mix.Sample(alpha) + 0.1f, Freq.Sample(alpha));
                        break;
                }
            }
        }
    }
}
