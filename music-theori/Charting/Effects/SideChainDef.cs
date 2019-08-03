using System;

using theori.Audio;
using theori.Audio.Effects;

namespace theori.Charting.Effects
{
    [EffectType("SideChain")]
    public sealed class SideChainDef : EffectDef
    {
        [TheoriProperty("amount")]
        public EffectParamF Amount;
        [TheoriProperty("duration")]
        public EffectParamF Duration;

        public SideChainDef() : base(1) { }
        
        public SideChainDef(EffectParamF mix, EffectParamF amount, EffectParamF dur)
            : base(mix)
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
            if (!(other is SideChainDef sch)) return false;
            return Mix == sch.Mix && Amount == sch.Amount && Duration == sch.Duration;
        }

        public override int GetHashCode() => HashCode.For(Mix, Amount, Duration);
    }
}
