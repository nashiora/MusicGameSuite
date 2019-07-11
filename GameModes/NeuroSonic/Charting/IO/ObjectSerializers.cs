using System.IO;

using theori;
using theori.Charting;
using theori.Charting.IO;

namespace NeuroSonic.Charting.IO
{
    public class ButtonObjectSerializer : ChartObjectSerializer<ButtonObject>
    {
        public override int ID => 1;

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
        public override int ID => 2;

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

    public class SlamVolumeEventSerializer : ChartObjectSerializer<SlamVolumeEvent>
    {
        public override int ID => StreamIndex.SlamVolume;

        public override ChartObject DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects)
        {
            var evt = new SlamVolumeEvent() { Position = pos };
            evt.Volume = reader.ReadSingleBE();
            return evt;
        }

        public override void SerializeSubclass(SlamVolumeEvent obj, BinaryWriter writer, ChartEffectTable effects)
        {
            writer.WriteSingleBE(obj.Volume);
        }
    }
}
