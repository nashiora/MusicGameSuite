namespace OpenRM.Voltex
{
    public class SpinImpulseEvent : Event
    {
        public SpinParams Params => new SpinParams()
        {
            Direction = Direction,
            Duration = AbsoluteDuration,
        };

        public AngularDirection Direction;
    }
    
    public class SwingImpulseEvent : Event
    {
        public SwingParams Params => new SwingParams()
        {
            Direction = Direction,
            Duration = AbsoluteDuration,
            Amplitude = Amplitude,
        };
        
        public AngularDirection Direction;
        public float Amplitude;
    }
    
    public class WobbleImpulseEvent : Event
    {
        public WobbleParams Params => new WobbleParams()
        {
            Direction = Direction,
            Duration = AbsoluteDuration,
            Amplitude = Amplitude,
            Frequency = Frequency,
            Decay = Decay,
        };
        
        public LinearDirection Direction;
        public float Amplitude;
        public int Frequency;
        public Decay Decay;
    }
}
