using System.IO;

namespace theori.Charting.IO
{
    // TODO(local): How can this best be instantiated based on desired game mode and file type?
    public interface IChartSerializer
    {
        string FormatExtension { get; }

        void SerializeChart(Chart chartInfo, Stream outStream);
        Chart DeserializeChart(ChartInfo chartInfo, Stream inStream);
    }
}
