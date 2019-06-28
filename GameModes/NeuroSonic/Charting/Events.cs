using theori.Audio.Effects;
using theori.Charting;

namespace NeuroSonic.Charting
{
    public class LaserApplicationEvent : ChartEvent
    {
        public LaserApplication Application = LaserApplication.Additive;
    }

    public class LaserParamsEvent : ChartEvent
    {
        public LaserIndex LaserIndex;
        public LaserParams Params;
    }

    public class PathPointEvent : ChartEvent
    {
        public float Value;
    }

    public class EffectKindEvent : ChartEvent
    {
        public int EffectIndex;
        public EffectDef Effect;
    }

    public class LaserFilterKindEvent : ChartEvent
    {
        public LaserIndex LaserIndex;
        public EffectDef FilterEffect;
    }

    public class LaserFilterGainEvent : ChartEvent
    {
        public LaserIndex LaserIndex;
        public float Gain;
    }

    public class SlamVolumeEvent : ChartEvent
    {
        public float Volume;
    }

    public class SpinImpulseEvent : ChartEvent
    {
        public SpinParams Params => new SpinParams()
        {
            Direction = Direction,
            Duration = AbsoluteDuration,
        };

        public AngularDirection Direction;
    }
    
    public class SwingImpulseEvent : ChartEvent
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
    
    public class WobbleImpulseEvent : ChartEvent
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
