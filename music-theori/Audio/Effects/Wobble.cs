using System;

namespace theori.Audio.Effects
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
            filter.Mix = 1.0f;
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
		        filter.Process(s, 0, 2);

		        // Apply slight mixing
		        float addMix = 0.85f;
                //buffer[i * 2 + 0] = buffer[i * 2 + 0] * addMix + s[0] * (1.0f - addMix);
                //buffer[i * 2 + 1] = buffer[i * 2 + 1] * addMix + s[1] * (1.0f - addMix);

                buffer[i * 2 + 0] = MathL.Lerp(buffer[i * 2 + 0], s[0], Mix * addMix);
                buffer[i * 2 + 1] = MathL.Lerp(buffer[i * 2 + 1], s[1], Mix * addMix);

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

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is Wobble wobble)
            {
                wobble.SetPeriod(Period.Sample(alpha) * qnDur.Seconds * 4);
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is WobbleEffectDef wob)) return false;
            return Type == wob.Type && Mix == wob.Mix && Period == wob.Period;
        }

        public override int GetHashCode() => HashCode.For(Type, Mix, Period);
    }
}
