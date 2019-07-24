using System;

using theori.Audio;
using theori.Audio.Effects;

namespace theori.Charting.Effects
{
    [EffectType("TapeStop")]
    public sealed class TapeStopDef : EffectDef
    {
        [TheoriProperty("duration")]
        public EffectParamF Duration;

        public TapeStopDef() : base(1) { }
        
        public TapeStopDef(EffectParamF mix, EffectParamF duration)
            : base(mix)
        {
            Duration = duration;
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new TapeStop(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is TapeStop ts)
            {
                ts.Duration = Duration.Sample(alpha);
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is TapeStopDef stop)) return false;
            return Mix == stop.Mix && Duration == stop.Duration;
        }

        public override int GetHashCode() => HashCode.For(Mix, Duration);
    }
}
