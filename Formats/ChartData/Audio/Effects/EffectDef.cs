using System;
using System.Collections.Generic;

namespace OpenRM.Audio.Effects
{
    public abstract class EffectDef
    {
        private static readonly Dictionary<EffectType, EffectDef> defaults =
            new Dictionary<EffectType, EffectDef>();

        public static EffectDef GetDefault(EffectType type)
        {
            if (!defaults.TryGetValue(type, out var result))
            {
                result = CreateDefault(type);
                defaults[type] = result;
            }
            return result;
        }

        private static EffectDef CreateDefault(EffectType type)
        {
            var laserEasingCurve = new CubicBezier(Ease.InExpo);
            var lpfEasingCurve = new CubicBezier(Ease.OutCubic);

            const float DEF_FILTER_GAIN = 1.0f;

            switch (type)
            {
                case EffectType.PeakingFilter:
                {
                    var q = new EffectParamF(1, 0.8f);
                    var freq = new EffectParamF(80, 8_000, laserEasingCurve);
                    float gain = 20.0f;

                    return new BiQuadFilterEffectDef(type, 1.0f, q, gain, freq);
                }
                    
                case EffectType.LowPassFilter:
                {
                    var q = new EffectParamF(7, 10);
                    var freq = new EffectParamF(10_000, 700, lpfEasingCurve);

                    return new BiQuadFilterEffectDef(type, 1.0f, q, DEF_FILTER_GAIN, freq);
                }
                    
                case EffectType.HighPassFilter:
                {
                    var q = new EffectParamF(10, 5);
                    var freq = new EffectParamF(80, 2_000, laserEasingCurve);

                    return new BiQuadFilterEffectDef(type, 1.0f, q, DEF_FILTER_GAIN, freq);
                }
                    
                case EffectType.BitCrush:
                {
                    var reduction = new EffectParamF(0, 45 / 44100.0f, laserEasingCurve);
                    return new BitCrusherEffectDef(1.0f, reduction);
                }

                default: throw new NotImplementedException(type.ToString());
            }
        }

        public EffectType Type { get; }
        public EffectParamF Mix { get; }

        protected EffectDef(EffectType type, EffectParamF mix)
        {
            Type = type;
            Mix = mix;
        }

        public abstract Dsp CreateEffectDsp(int sampleRate);
        public virtual void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            effect.Mix = Mix.Sample(alpha);
        }
    }
}
