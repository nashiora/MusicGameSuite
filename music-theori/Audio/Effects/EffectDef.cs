using System;

namespace theori.Audio.Effects
{
    public abstract class EffectDef : IEquatable<EffectDef>
    {
        public EffectParamF Mix;

        protected EffectDef() { }
        protected EffectDef(EffectParamF mix)
        {
            Mix = mix;
        }

        public abstract Dsp CreateEffectDsp(int sampleRate);
        public virtual void ApplyToDsp(Dsp effect, time_t qnDur, float alpha = 0)
        {
            effect.Mix = Mix.Sample(alpha);
        }

        public abstract bool Equals(EffectDef other);

        public override bool Equals(object obj)
        {
            if (obj is EffectDef def) return Equals(def);
            return false;
        }

        public override abstract int GetHashCode();
    }
}
