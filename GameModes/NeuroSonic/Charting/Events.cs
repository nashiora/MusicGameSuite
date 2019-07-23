using theori.Audio.Effects;
using theori.Charting;

namespace NeuroSonic.Charting
{
    public abstract class HighwayTypedEvent : ChartEvent { }
    public abstract class ButtonTypedEvent : ChartEvent { }
    public abstract class LaserTypedEvent : ChartEvent { }
    public abstract class CameraTypedEvent : ChartEvent { }

    [EntityType("LaserApplication")]
    public class LaserApplicationEvent : LaserTypedEvent
    {
        public LaserApplication Application = LaserApplication.Additive;
    }

    [EntityType("LaserParams")]
    public class LaserParamsEvent : LaserTypedEvent
    {
        public LaserIndex LaserIndex;
        public LaserParams Params;
    }

    [EntityType("PathPoint")]
    public class PathPointEvent : CameraTypedEvent
    {
        public float Value;
    }

    [EntityType("EffectKind")]
    public class EffectKindEvent : ButtonTypedEvent, IHasEffectDef
    {
        public int EffectIndex;
        public EffectDef Effect { get; set; }
    }

    [EntityType("LaserFilterKind")]
    public class LaserFilterKindEvent : LaserTypedEvent, IHasEffectDef
    {
        public LaserIndex LaserIndex;
        public EffectDef Effect { get; set; }
    }

    [EntityType("LaserFilterGain")]
    public class LaserFilterGainEvent : LaserTypedEvent
    {
        public LaserIndex LaserIndex;
        public float Gain;
    }

    [EntityType("SlamVolume")]
    public class SlamVolumeEvent : LaserTypedEvent
    {
        public float Volume;
    }

    [EntityType("SpinImpulse")]
    public class SpinImpulseEvent : HighwayTypedEvent
    {
        public SpinParams Params => new SpinParams()
        {
            Direction = Direction,
            Duration = AbsoluteDuration,
        };

        public AngularDirection Direction;
    }

    [EntityType("SwingImpulse")]
    public class SwingImpulseEvent : HighwayTypedEvent
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

    [EntityType("WobbleImpulse")]
    public class WobbleImpulseEvent : HighwayTypedEvent
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
