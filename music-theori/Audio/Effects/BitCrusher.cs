using System;

namespace theori.Audio.Effects
{
    public sealed class BitCrusher : Dsp
    {
        private double samplePosition;
        private double sampleScale;

        private float sampleLeft;
        private float sampleRight;

        public double Reduction = 4 / 44100.0f;

        public BitCrusher(int sampleRate)
            : base(sampleRate)
        {
            sampleScale = sampleRate / 44100.0f;
        }

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;

            for(int i = 0; i < numSamples; i++)
            {
                samplePosition += 1.0 / SampleRate;
                if(samplePosition > Reduction * sampleScale)
                {
                    sampleLeft = buffer[offset + i * 2];
                    sampleRight = buffer[offset + i * 2 + 1];
                    samplePosition -= Reduction * sampleScale;
                }

                buffer[offset + i * 2] = MathL.Lerp(buffer[offset + i * 2], sampleLeft, Mix);
                buffer[offset + i * 2 + 1] = MathL.Lerp(buffer[offset + i * 2 + 1], sampleRight, Mix);
            }
        }
    }
    
    public sealed class BitCrusherEffectDef : EffectDef
    {
        public EffectParamF Reduction { get; }
        
        public BitCrusherEffectDef(EffectParamF mix, EffectParamF reduction)
            : base(EffectType.BitCrush, mix)
        {
            Reduction = reduction;
        }
        
        public override Dsp CreateEffectDsp(int sampleRate = 0) => new BitCrusher(sampleRate);

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
            if (!(other is BitCrusherEffectDef bc)) return false;
            return Type == bc.Type && Mix == bc.Mix && Reduction == bc.Reduction;
        }

        public override int GetHashCode() => HashCode.For(Type, Mix, Reduction);
    }
}
