using System;

using theori.Audio;
using theori.Audio.Effects;

namespace theori.Charting.Effects
{
    [EffectType("Gate")]
    public sealed class GateDef : EffectDef
    {
        [TheoriProperty("gateDuration")]
        public EffectParamF GateDuration = new EffectParamX(16);
        [TheoriProperty("gating")]
        public EffectParamF Gating = 0.6f;

        public GateDef() : base(1) { }
        public GateDef(EffectParamF mix, EffectParamF gating, EffectParamF gateDuration)
            : base(mix)
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
            if (!(other is GateDef rt)) return false;
            return Mix == rt.Mix && GateDuration == rt.GateDuration && Gating == rt.Gating;
        }

        public override int GetHashCode() => HashCode.For(Mix, GateDuration, Gating);
    }
}
