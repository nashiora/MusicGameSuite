namespace OpenRM.Audio.Effects
{
    public sealed class SideChain : Dsp
    {
        private double time;

        public float Amount { get; set; } = 1.0f;

        public double Duration { get; set; } = 0.5;

        public SideChain(int sampleRate)
            : base(sampleRate)
        {
        }
        
        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;
            if(Duration == 0.0)
                return;

            double step = 1.0 / SampleRate;
            for(int i = 0; i < numSamples; i++)
            {
                float r = (float)(time / Duration);
                // FadeIn
                const float fadeIn = 0.05f;
                if(r < fadeIn)
                    r = 1.0f - r / fadeIn;
                else r = Curve((r - fadeIn) / (1.0f - fadeIn));
                float sampleGain = 1.0f - Amount * (1.0f - r);
                buffer[offset + i * 2 + 0] *= sampleGain;
                buffer[offset + i * 2 + 1] *= sampleGain;

                time += step;
                if(time > Duration)
                    time = 0;
            }
        }

        private float Curve(float input)
        {
            return input;
        }
    }

    public sealed class SideChainEffectDef : EffectDef
    {
        public EffectParamF Amount { get; }
        public new EffectParamF Duration { get; }
        
        public SideChainEffectDef(EffectParam<EffectDuration> duration, EffectParamF mix,
            EffectParamF amount, EffectParamF dur)
            : base(EffectType.SideChain, duration, mix)
        {
            Amount = amount;
            Duration = dur;
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new SideChain(sampleRate);

        public override void ApplyToDsp(Dsp effect, float alpha = 0)
        {
            if (effect is SideChain sc)
            {
                sc.Amount = Amount.Sample(alpha);
                sc.Duration = Duration.Sample(alpha);
            }
        }
    }
}
