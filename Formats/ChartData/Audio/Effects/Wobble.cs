using System;

namespace OpenRM.Audio.Effects
{
    public class Wobble : Dsp
    {
	    private static readonly CubicBezier easing = new CubicBezier(Ease.InExpo);

        private BiQuadFilter filter;

	    // Frequency range
	    float fmin = 500.0f;
	    float fmax = 20000.0f;
	    float q = 1.414f;

        private int m_currentSample, m_length;

        public Wobble(int sampleRate)
            : base(sampleRate)
        {
            filter = new BiQuadFilter(sampleRate);
        }
        
        public void SetPeriod(double period)
        {
	        m_length = (int)(period * SampleRate);
        }

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;
            
	        for(int i = 0; i < numSamples; i++)
	        {
		        float f = MathL.Abs(2.0f * ((float)m_currentSample / m_length) - 1.0f);
		        f = easing.Sample(f);
		        float freq = fmin + (fmax - fmin) * f;
		        filter.SetLowPass(q, freq);

		        float[] s = { buffer[i * 2], buffer[i * 2 + 1] };
		        filter.Process(buffer, i * 2, 2);

		        // Apply slight mixing
		        float mix = 0.85f;
		        buffer[i * 2 + 0] = buffer[i * 2 + 0] * mix + s[0] * (1.0f - mix);
		        buffer[i * 2 + 1] = buffer[i * 2 + 1] * mix + s[1] * (1.0f - mix);

		        m_currentSample++;
		        m_currentSample %= m_length;
	        }
        }
    }

    public class WobbleEffectDef : EffectDef
    {
        public EffectParamF Period;

        public WobbleEffectDef(EffectParamF mix, EffectParamF period)
            : base(EffectType.Wobble, mix)
        {
            Period = period;
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new Wobble(sampleRate);

        public override void ApplyToDsp(Dsp effect, float alpha = 0)
        {
            base.ApplyToDsp(effect, alpha);
            if (effect is Wobble wobble)
            {
                wobble.SetPeriod(Period.Sample(alpha));
            }
        }
    }
}
