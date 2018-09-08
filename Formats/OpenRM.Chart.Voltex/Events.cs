using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRM.Audio.Effects;

namespace OpenRM.Voltex
{
    public class LaserApplicationEvent : Event
    {
        public LaserApplication Application = LaserApplication.Additive;
    }

    public class LaserParamsEvent : Event
    {
        public LaserIndex LaserIndex;
        public LaserParams Params;
    }

    public class PathPointEvent : Event
    {
        public float Value;
    }

    public class LaserFilterKindEvent : Event
    {
        public LaserIndex LaserIndex;
        public EffectDef FilterEffect;
    }

    public class LaserFilterGainEvent : Event
    {
        public LaserIndex LaserIndex;
        public float Gain;
    }

    public class SlamVolumeEvent : Event
    {
        public float Volume;
    }

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
