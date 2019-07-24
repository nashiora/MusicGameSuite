using System;

using theori.Audio;
using theori.Audio.Effects;

namespace theori.Charting.Effects
{
    public sealed class RetriggerDef : EffectDef
    {
        public EffectParamF GateDuration;
        public EffectParamF Gating;

        public RetriggerDef() : base(1) { }
        
        public RetriggerDef(EffectParamF mix, EffectParamF gating, EffectParamF gateDuration)
            : base(mix)
        {
            GateDuration = gateDuration;
            Gating = gating;
        }
        
        public override Dsp CreateEffectDsp(int sampleRate) => new Retrigger(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is Retrigger retrigger)
            {
                retrigger.Mix = Mix.Sample(alpha);
                retrigger.Gating = Gating.Sample(alpha);
                retrigger.Duration = GateDuration.Sample(alpha) * qnDur.Seconds * 4;
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is RetriggerDef rt)) return false;
            return Mix == rt.Mix && GateDuration == rt.GateDuration && Gating == rt.Gating;
        }

        public override int GetHashCode() => HashCode.For(Mix, GateDuration, Gating);
    }
}
