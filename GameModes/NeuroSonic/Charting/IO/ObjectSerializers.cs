using System.Diagnostics;
using System.IO;

using theori;
using theori.Audio.Effects;
using theori.Charting;
using theori.Charting.IO;

namespace NeuroSonic.Charting.IO
{
    public class ButtonObjectSerializer : ChartObjectSerializer<ButtonObject>
    {
        public ButtonObjectSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            byte flags = reader.ReadUInt8();
            bool hasSample = flags != 0;

            var obj = new ButtonObject() { Position = pos, Duration = dur };
            if (hasSample) obj.Sample = reader.ReadStringUTF8();

            return obj;
        }

        public override void SerializeSubclass(ButtonObject obj, BinaryWriter writer, ChartEffectTable effects)
        {
            if (obj.HasSample)
            {
                writer.WriteUInt8(0x01);
                writer.WriteStringUTF8(obj.Sample);
            }
            else writer.WriteUInt8(0);
        }
    }

    public class AnalogObjectSerializer : ChartObjectSerializer<AnalogObject>
    {
        public AnalogObjectSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            byte flags = reader.ReadUInt8();

            var obj = new AnalogObject() { Position = pos, Duration = dur };
            if ((flags & 0x01) != 0) obj.RangeExtended = true;

            obj.InitialValue = reader.ReadSingleBE();
            obj.FinalValue = reader.ReadSingleBE();
            obj.Shape = (CurveShape)reader.ReadUInt8();

            switch (obj.Shape)
            {
                case CurveShape.Linear: break;

                case CurveShape.Cosine:
                case CurveShape.ThreePoint:
                    obj.CurveA = reader.ReadSingleBE();
                    obj.CurveB = reader.ReadSingleBE();
                    break;
            }

            return obj;
        }

        public override void SerializeSubclass(AnalogObject obj, BinaryWriter writer, ChartEffectTable effects)
        {
            byte flags = 0;
            if (obj.RangeExtended) flags |= 0x01;

            writer.WriteUInt8(flags);
            writer.WriteSingleBE(obj.InitialValue);
            writer.WriteSingleBE(obj.FinalValue);
            writer.WriteUInt8((byte)obj.Shape);

            switch (obj.Shape)
            {
                case CurveShape.Linear: break;

                case CurveShape.Cosine:
                case CurveShape.ThreePoint:
                    writer.WriteSingleBE(obj.CurveA);
                    writer.WriteSingleBE(obj.CurveB);
                    break;
            }
        }
    }

    public class LaserApplicationEventSerializer : ChartObjectSerializer<LaserApplicationEvent>
    {
        public LaserApplicationEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            return new LaserApplicationEvent() { Position = pos, Application = (LaserApplication)reader.ReadUInt8() };
        }

        public override void SerializeSubclass(LaserApplicationEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            writer.WriteUInt8((byte)obj.Application);
        }
    }

    public class LaserParamsEventSerializer : ChartObjectSerializer<LaserParamsEvent>
    {
        public LaserParamsEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            var evt = new LaserParamsEvent() { Position = pos };
            evt.LaserIndex = (LaserIndex)reader.ReadUInt8();
            evt.Params.Function = (LaserFunction)reader.ReadUInt8();
            evt.Params.Scale = (LaserScale)reader.ReadUInt8();
            return evt;
        }

        public override void SerializeSubclass(LaserParamsEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            writer.WriteUInt8((byte)obj.LaserIndex);
            writer.WriteUInt8((byte)obj.Params.Function);
            writer.WriteUInt8((byte)obj.Params.Scale);
        }
    }

    public class PathPointEventSerializer : ChartObjectSerializer<PathPointEvent>
    {
        public PathPointEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            return new PathPointEvent() { Position = pos, Value = reader.ReadSingleBE() };
        }

        public override void SerializeSubclass(PathPointEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            writer.WriteSingleBE(obj.Value);
        }
    }

    public class EffectKindEventSerializer : ChartObjectSerializer<EffectKindEvent>
    {
        public EffectKindEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            int laneIndex = reader.ReadUInt8();
            ushort effectID = reader.ReadUInt16BE();

            EffectDef effect;
            if (effectID == ushort.MaxValue)
                effect = null;
            else effect = effects[effectID];

            return new EffectKindEvent() { Position = pos, EffectIndex = laneIndex, Effect = effect };
        }

        public override void SerializeSubclass(EffectKindEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            int effectID = effects.IndexOf(obj.Effect);
            if (effectID < 0) effectID = ushort.MaxValue;
            //Debug.Assert(effectIndex >= 0, "Failed to properly save effect to table, couldn't find");

            writer.WriteUInt8((byte)obj.EffectIndex);
            writer.WriteUInt16BE((ushort)effectID);
        }
    }

    public class LaserFilterKindEventSerializer : ChartObjectSerializer<LaserFilterKindEvent>
    {
        public LaserFilterKindEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            var laserIndex = (LaserIndex)reader.ReadUInt8();
            ushort effectID = reader.ReadUInt16BE();

            EffectDef effect;
            if (effectID == ushort.MaxValue)
                effect = null;
            else effect = effects[effectID];

            return new LaserFilterKindEvent() { Position = pos, LaserIndex = laserIndex, Effect = effect };
        }

        public override void SerializeSubclass(LaserFilterKindEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            int effectID = effects.IndexOf(obj.Effect);
            if (effectID < 0) effectID = ushort.MaxValue;
            //Debug.Assert(effectIndex >= 0, "Failed to properly save effect to table, couldn't find");

            writer.WriteUInt8((byte)obj.LaserIndex);
            writer.WriteUInt16BE((ushort)effectID);
        }
    }

    public class LaserFilterGainEventSerializer : ChartObjectSerializer<LaserFilterGainEvent>
    {
        public LaserFilterGainEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            var laserIndex = (LaserIndex)reader.ReadUInt8();
            float gain = reader.ReadSingleBE();
            return new LaserFilterGainEvent() { Position = pos, LaserIndex = laserIndex, Gain = gain };
        }

        public override void SerializeSubclass(LaserFilterGainEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            writer.WriteUInt8((byte)obj.LaserIndex);
            writer.WriteSingleBE(obj.Gain);
        }
    }

    public class SlamVolumeEventSerializer : ChartObjectSerializer<SlamVolumeEvent>
    {
        public SlamVolumeEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            return new SlamVolumeEvent() { Position = pos, Volume = reader.ReadSingleBE() };
        }

        public override void SerializeSubclass(SlamVolumeEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            writer.WriteSingleBE(obj.Volume);
        }
    }

    public class SpinImpulseEventSerializer : ChartObjectSerializer<SpinImpulseEvent>
    {
        public SpinImpulseEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            return new SpinImpulseEvent() { Position = pos, Direction = (AngularDirection)reader.ReadUInt8() };
        }

        public override void SerializeSubclass(SpinImpulseEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            writer.WriteUInt8((byte)obj.Direction);
        }
    }

    public class SwingImpulseEventSerializer : ChartObjectSerializer<SwingImpulseEvent>
    {
        public SwingImpulseEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            var evt = new SwingImpulseEvent() { Position = pos };
            evt.Direction = (AngularDirection)reader.ReadUInt8();
            evt.Amplitude = reader.ReadSingleBE();
            return evt;
        }

        public override void SerializeSubclass(SwingImpulseEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            writer.WriteUInt8((byte)obj.Direction);
            writer.WriteSingleBE(obj.Amplitude);
        }
    }

    public class WobbleImpulseEventSerializer : ChartObjectSerializer<WobbleImpulseEvent>
    {
        public WobbleImpulseEventSerializer(int id) : base(id) { }

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            var evt = new WobbleImpulseEvent() { Position = pos };
            evt.Direction = (LinearDirection)reader.ReadUInt8();
            evt.Amplitude = reader.ReadSingleBE();
            evt.Frequency = reader.ReadUInt16BE();
            evt.Decay = (Decay)reader.ReadUInt8();
            return evt;
        }

        public override void SerializeSubclass(WobbleImpulseEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            writer.WriteUInt8((byte)obj.Direction);
            writer.WriteSingleBE(obj.Amplitude);
            writer.WriteUInt16BE((ushort)obj.Frequency);
            writer.WriteUInt8((byte)obj.Decay);
        }
    }
}
