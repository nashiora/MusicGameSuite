namespace OpenRM.Audio.Effects
{
    public sealed class BitCrusher : Dsp
    {
        private double samplePosition;

        private float sampleLeft;
        private float sampleRight;

        /// <summary>
        /// Make larger than 1 to stretch out samples across a longer period of samples
        /// </summary>
        public double Reduction = 4;

        public BitCrusher()
            : base(0)
        {
        }

        protected override void ProcessImpl(float[] buffer, int offset, int count)
        {
            int numSamples = count / 2;

            for(int i = 0; i < numSamples; i++)
            {
                samplePosition += 1.0;
                if(samplePosition > Reduction)
                {
                    sampleLeft = buffer[offset + i * 2];
                    sampleRight = buffer[offset + i * 2 + 1];
                    samplePosition -= Reduction;
                }

                buffer[offset + i * 2] = sampleLeft;
                buffer[offset + i * 2 + 1] = sampleRight;
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
        
        public override Dsp CreateEffectDsp(int sampleRate = 0) => new BitCrusher();

        public override void ApplyToDsp(Dsp effect, float alpha = 0)
        {
            base.ApplyToDsp(effect, alpha);
            if (effect is BitCrusher bitCrusher)
            {
                bitCrusher.Reduction = Reduction.Sample(alpha);
            }
        }
    }
}
