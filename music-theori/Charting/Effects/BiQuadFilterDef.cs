using System;

using theori.Audio;
using theori.Audio.Effects;

namespace theori.Charting.Effects
{
    public enum FilterType
    {
        Peak, LowPass, HighPass
    }

    [EffectType("BiQuad")]
    public sealed class BiQuadFilterDef : EffectDef
    {
        public static BiQuadFilterDef CreateDefaultPeak()
        {
            var q = new EffectParamF(1, 0.8f, Ease.Linear);
            var freq = new EffectParamF(80, 8_000, Ease.InExpo);
            float gain = 20.0f;
            return new BiQuadFilterDef(FilterType.Peak, 1.0f, q, gain, freq);
        }

        public static BiQuadFilterDef CreateDefaultLowPass()
        {
            var q = new EffectParamF(7, 10, Ease.Linear);
            var freq = new EffectParamF(10_000, 700, Ease.OutCubic);
            return new BiQuadFilterDef(FilterType.LowPass, 1.0f, q, 1, freq);
        }

        public static BiQuadFilterDef CreateDefaultHighPass()
        {
            var q = new EffectParamF(10, 5, Ease.Linear);
            var freq = new EffectParamF(80, 2_000, Ease.InExpo);
            return new BiQuadFilterDef(FilterType.HighPass, 1.0f, q, 1, freq);
        }

        [TheoriProperty("filterType")]
        public FilterType FilterType;

        [TheoriProperty("q")]
        public EffectParamF Q;
        [TheoriProperty("gain")]
        public EffectParamF Gain;
        [TheoriProperty("frequency")]
        public EffectParamF Freq;

        public BiQuadFilterDef() : base(1) { }
        public BiQuadFilterDef(FilterType type, EffectParamF mix,
            EffectParamF q, EffectParamF gain, EffectParamF freq)
            : base(mix)
        {
            FilterType = type;
            Q = q;
            Gain = gain;
            Freq = freq;
        }

        public override Dsp CreateEffectDsp(int sampleRate) => new BiQuadFilter(sampleRate);

        public override void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            base.ApplyToDsp(effect, qnDur, alpha);
            if (effect is BiQuadFilter filter)
            {
                switch (FilterType)
                {
                    case FilterType.Peak:
                        filter.SetPeaking(Q.Sample(alpha), Freq.Sample(alpha), Gain.Sample(alpha));
                        break;
                        
                    case FilterType.LowPass:
                        filter.SetLowPass(Q.Sample(alpha) * Mix.Sample(alpha) + 0.1f, Freq.Sample(alpha));
                        break;
                        
                    case FilterType.HighPass:
                        filter.SetHighPass(Q.Sample(alpha) * Mix.Sample(alpha) + 0.1f, Freq.Sample(alpha));
                        break;
                }
            }
        }

        public override bool Equals(EffectDef other)
        {
            if (!(other is BiQuadFilterDef bqf)) return false;
            return FilterType == bqf.FilterType && Mix == bqf.Mix && Q == bqf.Q && Gain == bqf.Gain && Freq == bqf.Freq;
        }

        public override int GetHashCode() => HashCode.For(FilterType, Mix, Q, Gain, Freq);
    }
}
