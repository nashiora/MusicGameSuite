using System;

using theori.Audio;
using theori.Audio.Effects;

namespace theori.Charting.Effects
{
    [EffectType("Flanger")]
    public class FlangerDef : EffectDef
    {
        [TheoriProperty("delay")]
        public EffectParamF Delay = 2.0f;
        [TheoriProperty("offset")]
        public EffectParamI Offset = 10;
        [TheoriProperty("depth")]
        public EffectParamI Depth = 40;

        public FlangerDef() : base(1) { }
        public FlangerDef(EffectParamF mix)
            : base(mix)
        {
        }

        public FlangerDef(EffectParamF mix, EffectParamF delay, EffectParamI offset, EffectParamI depth)
            : this(mix)
        {
            Delay = delay;
            Offset = offset;
            Depth = depth;
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new Flanger(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is Flanger flanger)
            {
                flanger.SetDelay(Delay.Sample(alpha));
                flanger.SetDelayRange(Offset.Sample(alpha), Depth.Sample(alpha));
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is FlangerDef fl)) return false;
            return Mix == fl.Mix && Delay == fl.Delay && Offset == fl.Offset && Depth == fl.Depth;
        }

        public override int GetHashCode() => HashCode.For(Mix, Delay, Offset, Depth);
    }
}
