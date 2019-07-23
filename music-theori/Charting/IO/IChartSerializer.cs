using System.IO;

namespace theori.Charting.IO
{
    // TODO(local): How can this best be instantiated based on desired game mode and file type?
    public interface IChartSerializer
    {
        string FormatExtension { get; }
        string ParentDirectory { get; }

        void SaveToFile(Chart chart);
        Chart LoadFromFile(ChartInfo chartInfo);
    }
}
