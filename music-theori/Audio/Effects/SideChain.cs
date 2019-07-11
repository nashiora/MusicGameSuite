using System;

namespace theori.Audio.Effects
{
    public sealed class SideChain : Dsp
    {
        private static CubicBezier Curve = new CubicBezier(0.39f, 0.575f, 0.565f, 1);

        private double time;

        public float Amount = 1.0f;
        public double Duration = 0.5;

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
                const float fadeIn = 0.08f;
                if(r < fadeIn)
                    r = 1.0f - r / fadeIn;
                else r = Curve.Sample((r - fadeIn) / (1.0f - fadeIn));
                float sampleGain = 1.0f - Amount * (1.0f - r) * Mix;
                buffer[offset + i * 2 + 0] *= sampleGain;
                buffer[offset + i * 2 + 1] *= sampleGain;

                time += step;
                if(time > Duration)
                    time = 0;
            }
        }
    }

    public sealed class SideChainEffectDef : EffectDef
    {
        public EffectParamF Amount { get; }
        public EffectParamF Duration { get; }
        
        public SideChainEffectDef(EffectParamF mix, EffectParamF amount, EffectParamF dur)
            : base(EffectType.SideChain, mix)
        {
            Amount = amount;
            Duration = dur;
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new SideChain(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is SideChain sc)
            {
                sc.Amount = Amount.Sample(alpha);
                sc.Duration = Duration.Sample(alpha) * qnDur.Seconds * 4;
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is SideChainEffectDef sch)) return false;
            return Type == sch.Type && Mix == sch.Mix && Amount == sch.Amount && Duration == sch.Duration;
        }

        public override int GetHashCode() => HashCode.For(Type, Mix, Amount, Duration);
    }
}
