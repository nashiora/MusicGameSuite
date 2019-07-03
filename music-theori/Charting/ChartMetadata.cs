using System.Numerics;

namespace theori.Charting
{
    public class ChartSetMetadata
    {
        public string SongTitle { get; set; } = "Unknown";
        public string SongArtist { get; set; } = "Unknown";
        public string SongFileName { get; set; }
    }

    public class ChartMetadata
    {
        public string Charter { get; set; } = "Unknown";

        public string JacketFileName { get; set; }
        public string JacketArtist { get; set; }

        public string BackgroundFileName { get; set; }
        public string BackgroundArtist { get; set; }

        public double DifficultyLevel { get; set; } = 1.0;
        public int? DifficultyIndex { get; set; }

        public string DifficultyName { get; set; }
        public string DifficultyNameShort { get; set; }

        public Vector3? DifficultyColor { get; set; } = null;

        public time_t ChartDuration { get; set; } = 0;
    }
}
