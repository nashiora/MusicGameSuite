namespace OpenRM.Audio.Effects
{
    public enum EffectDurationKind
    {
        Rate,
        Tick,
    }

    public class EffectDuration
    {
        public static readonly EffectDuration Zero = new EffectDuration();

        public EffectDurationKind Kind { get; private set; }

        private float rate;
        public float Rate
        {
            get => rate;
            set
            {
                rate = value;
                Kind = EffectDurationKind.Rate;
            }
        }

        private int tick;
        public int Tick
        {
            get => tick;
            set
            {
                tick = value;
                Kind = EffectDurationKind.Tick;
            }
        }

        public EffectDuration(int duration = 0)
        {
            Tick = duration;
        }

        public EffectDuration(float rate)
        {
            Rate = rate;
        }

        public int Absolute(int noteDuration)
        {
            throw new System.NotImplementedException();
        }
    }
}
