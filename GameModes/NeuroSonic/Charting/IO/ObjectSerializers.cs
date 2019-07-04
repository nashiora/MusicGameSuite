using theori.Charting.IO;

namespace NeuroSonic.Charting.IO
{
    public class ButtonObjectSerializer : ChartObjectSerializer<ButtonObject>
    {
        public override int ID => 1;

        public override string SerializeSubclass(ButtonObject obj)
        {
            if (obj.HasSample)
                return $"\"{obj.Sample}\"";
            else return "0";
        }
    }

    public class AnalogObjectSerializer : ChartObjectSerializer<AnalogObject>
    {
        public override int ID => 2;

        public override string SerializeSubclass(AnalogObject obj)
        {
            int extState = obj.RangeExtended ? 1 : 0;
            return $"{obj.InitialValue},{obj.FinalValue},{extState},{obj.Shape},{obj.CurveA},{obj.CurveB}";
        }
    }
}
