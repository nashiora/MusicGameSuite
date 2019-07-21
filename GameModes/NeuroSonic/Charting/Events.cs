using theori.Audio.Effects;
using theori.Charting;

namespace NeuroSonic.Charting
{
    [ChartObjectType("LaserApplication")]
    public class LaserApplicationEvent : ChartEvent
    {
        public LaserApplication Application = LaserApplication.Additive;
    }

    [ChartObjectType("LaserParams")]
    public class LaserParamsEvent : ChartEvent
    {
        public LaserIndex LaserIndex;
        public LaserParams Params;
    }

    [ChartObjectType("PathPoint")]
    public class PathPointEvent : ChartEvent
    {
        public float Value;
    }

    [ChartObjectType("EffectKind")]
    public class EffectKindEvent : ChartEvent, IHasEffectDef
    {
        public int EffectIndex;
        public EffectDef Effect { get; set; }
    }

    [ChartObjectType("LaserFilterKind")]
    public class LaserFilterKindEvent : ChartEvent, IHasEffectDef
    {
        public LaserIndex LaserIndex;
        public EffectDef Effect { get; set; }
    }

    [ChartObjectType("LaserFilterGain")]
    public class LaserFilterGainEvent : ChartEvent
    {
        public LaserIndex LaserIndex;
        public float Gain;
    }

    [ChartObjectType("SlamVolume")]
    public class SlamVolumeEvent : ChartEvent
    {
        public float Volume;
    }

    [ChartObjectType("SpinImpulse")]
    public class SpinImpulseEvent : ChartEvent
    {
        public SpinParams Params => new SpinParams()
        {
            Direction = Direction,
            Duration = AbsoluteDuration,
        };

        public AngularDirection Direction;
    }

    [ChartObjectType("SwingImpulse")]
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

    [ChartObjectType("WobbleImpulse")]
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
