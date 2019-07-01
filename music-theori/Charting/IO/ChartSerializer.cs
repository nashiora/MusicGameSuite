using System.IO;
using theori.GameModes;

namespace theori.Charting.IO
{
    public class ChartSerializer
    {
        public static ChartSerializer GetDefaultSerializer()
        {
            return new ChartSerializer();
        }

        public static ChartSerializer GetSerializerFor<T>()
            where T : GameMode
        {
            //return Host.GetSharedGameMode<T>().CreateChartSerializer() ?? GetDefaultSerializer();
            return null;
        }

        protected ChartSerializer()
        {
        }

        public virtual void Serialize(Chart chart, Stream outStream)
        {
        }
    }
}
