using System;

using theori.Audio;
using theori.Audio.Effects;

namespace theori.Charting.Effects
{
    [EffectType("Phaser")]
    public sealed class PhaserDef : EffectDef
    {
        public PhaserDef() : base(1) { }
        public PhaserDef(EffectParamF mix)
            : base(mix)
        {
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new Phaser(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is Phaser p)
            {
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is PhaserDef ph)) return false;
            return Mix == ph.Mix;
        }

        public override int GetHashCode() => HashCode.For(Mix);
    }
}
