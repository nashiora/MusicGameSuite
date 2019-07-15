using System.IO;

namespace theori.Charting.IO
{
    // TODO(local): How can this best be instantiated based on desired game mode and file type?
    public interface IChartSerializer
    {
        string FormatExtension { get; }

        void SaveToFile(string parentDirectory, Chart chart);
        Chart LoadFromFile(string parentDirectory, ChartInfo chartInfo);
    }
}
