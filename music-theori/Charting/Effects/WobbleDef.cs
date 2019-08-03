using System;

using theori.Audio;
using theori.Audio.Effects;

namespace theori.Charting.Effects
{
    [EffectType("Wobble")]
    public class WobbleDef : EffectDef
    {
        [TheoriProperty("period")]
        public EffectParamF Period = 0.25f;

        public WobbleDef() : base(1) { }

        public WobbleDef(EffectParamF mix, EffectParamF period)
            : base(mix)
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
            if (!(other is WobbleDef wob)) return false;
            return Mix == wob.Mix && Period == wob.Period;
        }

        public override int GetHashCode() => HashCode.For(Mix, Period);
    }
}
