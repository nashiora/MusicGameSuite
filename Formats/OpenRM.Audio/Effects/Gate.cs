using System;

namespace OpenRM.Audio.Effects
{
    public sealed class Gate : Dsp, IMixable
    {
	    public float LowVolume = 0.1f;

	    float m_gating = 0.75f;
	    uint m_length = 0;
	    uint m_fadeIn = 0; // Fade In mark
	    uint m_fadeOut = 0; // Fade Out mark
	    uint m_halfway; // Halfway mark
	    uint m_currentSample = 0;

        public Gate(int sampleRate)
            : base(sampleRate)
        {
        }
        
        public void SetLength(time_t length)
        {
	        float flength = (float)(length.Seconds * SampleRate);
	        m_length = (uint)flength;
	        SetGating(m_gating);
        }

        public void SetGating(float gating)
        {
	        float flength = m_length;
	        m_gating = gating;
	        m_halfway = (uint)(flength * gating);
	        float fadeDuration = MathL.Min(0.05f, gating * 0.5f);
	        m_fadeIn = (uint)(m_halfway * fadeDuration);
	        m_fadeOut = (uint)(m_halfway * (1.0f - fadeDuration));
	        m_currentSample = 0;
        }

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;

            for(int i = 0; i < numSamples; i++)
            {
                float c = 1.0f;
		        if(m_currentSample < m_halfway)
		        {
			        // Fade out before silence
			        if(m_currentSample > m_fadeOut)
				        c = 1-(float)(m_currentSample - m_fadeOut) / (float)m_fadeIn;
		        }
		        else
		        {
			        uint t = m_currentSample - m_halfway;
			        // Fade in again
			        if(t > m_fadeOut)
				        c = (float)(t - m_fadeOut) / (float)m_fadeIn;
			        else
				        c = 0.0f;
		        }

		        // Multiply volume
		        c = (c * (1 - LowVolume) + LowVolume); // Range [low, 1]
		        c = c * Mix + (1 - Mix);
		        buffer[i * 2] *= c;
		        buffer[i * 2 + 1] *= c;

		        m_currentSample++;
		        m_currentSample %= m_length;
            }
        }
    }

    public sealed class GateEffectDef : EffectDef
    {
        public EffectParamF Gating { get; }

        public GateEffectDef(EffectParam<EffectDuration> duration, EffectParamF mix,
            EffectParamF gating)
            : base(EffectType.Retrigger, duration, mix)
        {
            Gating = gating;
        }
        
        public override Dsp CreateEffectDsp(int sampleRate) => new Gate(sampleRate);

        public override void ApplyToDsp(Dsp effect, float alpha = 0)
        {
            if (effect is Gate gate)
            {
                gate.SetGating(Gating.Sample(alpha));
            }
        }
    }
}
