using System;

namespace theori.Audio.Effects
{
    public sealed class Gate : Dsp, IMixable
    {
	    private float m_lowVolume = 0.1f;
	    private float m_gating = 0.75f;
	    private uint m_gateDuration = 0;
	    private uint m_fadeIn = 0; // Fade In mark
	    private uint m_fadeOut = 0; // Fade Out mark
	    private uint m_halfway; // Halfway mark
	    private uint m_currentSample = 0;

        public Gate(int sampleRate)
            : base(sampleRate)
        {
        }
        
        public void SetGateDuration(double gateDuration)
        {
	        m_gateDuration = (uint)(gateDuration * SampleRate);
	        SetGating(m_gating);
        }

        public void SetGating(float gating)
        {
	        m_gating = gating;
	        m_halfway = (uint)(m_gateDuration * gating);

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
				        c = 1 - (m_currentSample - m_fadeOut) / m_fadeIn;
		        }
		        else
		        {
			        // Fade in again
			        uint t = m_currentSample - m_halfway;
			        if(t > m_fadeOut)
				        c = (t - m_fadeOut) / m_fadeIn;
			        else c = 0.0f;
		        }

		        // Multiply volume
		        c = c * (1 - m_lowVolume) + m_lowVolume; // Range [low, 1]
		        c = c * Mix + (1 - Mix);
		        buffer[i * 2] *= c;
		        buffer[i * 2 + 1] *= c;

		        m_currentSample++;
		        m_currentSample %= m_gateDuration;
            }
        }
    }

    public sealed class GateEffectDef : EffectDef
    {
        public EffectParamF GateDuration { get; }
        public EffectParamF Gating { get; }

        public GateEffectDef(EffectParamF mix, EffectParamF gating, EffectParamF gateDuration)
            : base(EffectType.Gate, mix)
        {
            GateDuration = gateDuration;
            Gating = gating;
        }
        
        public override Dsp CreateEffectDsp(int sampleRate) => new Gate(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is Gate gate)
            {
                gate.SetGating(Gating.Sample(alpha));
                gate.SetGateDuration(GateDuration.Sample(alpha) * qnDur.Seconds * 4);
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is GateEffectDef rt)) return false;
            return Type == rt.Type && Mix == rt.Mix && GateDuration == rt.GateDuration && Gating == rt.Gating;
        }

        public override int GetHashCode() => HashCode.For(Type, Mix, GateDuration, Gating);
    }
}
