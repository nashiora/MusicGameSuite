using System;

using theori.Audio;
using theori.Audio.Effects;

namespace theori.Charting.Effects
{
    [EffectType("BitCrusher")]
    public sealed class BitCrusherDef : EffectDef
    {
        [TheoriProperty("reduction")]
        public EffectParamI Reduction = 4;

        public BitCrusherDef() : base(1) { }
        
        public BitCrusherDef(EffectParamF mix, EffectParamI reduction)
            : base(mix)
        {
            Reduction = reduction;
        }
        
        public override Dsp CreateEffectDsp(int sampleRate) => new BitCrusher(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is BitCrusher bitCrusher)
            {
                bitCrusher.Reduction = Reduction.Sample(alpha);
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is BitCrusherDef bc)) return false;
            return Mix == bc.Mix && Reduction == bc.Reduction;
        }

        public override int GetHashCode() => HashCode.For(Mix, Reduction);
    }
}
