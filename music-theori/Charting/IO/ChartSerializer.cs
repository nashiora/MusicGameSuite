using System;
using System.IO;
using theori.GameModes;

namespace theori.Charting.IO
{
    public class ChartSerializer
    {
        private const int VERSION = 1;

        public int Version => VERSION;

        public static ChartSerializer GetDefaultSerializer()
        {
            return new ChartSerializer();
        }

        public static ChartSerializer GetSerializerFor(GameMode gameMode)
        {
            return new ChartSerializer(gameMode);
        }

        public static ChartSerializer GetSerializerForShared<T>()
            where T : GameMode
        {
            //return Host.GetSharedGameMode<T>().CreateChartSerializer() ?? GetDefaultSerializer();
            return null;
        }

        private readonly GameMode m_mode;

        private ChartSerializer(GameMode mode = null)
        {
            m_mode = mode;
        }

        public void SerializeChart(Chart chart, Stream outStream)
        {
            var writer = new StreamWriter(outStream);

            writer.WriteLine("#version " + Version);
            writer.WriteLine();
            writer.WriteLine("#control-points");
            foreach (var point in chart.ControlPoints)
                writer.WriteLine($"{(double)point.Position} {point.BeatsPerMinute} {point.BeatCount}/{point.BeatKind} {point.SpeedMultiplier}");

            for (int i = 0; i < chart.StreamCount; i++)
            {
                var stream = chart[i];

                writer.WriteLine();
                writer.WriteLine("#stream " + i);
            }

            writer.Flush();
        }

        public void SerializeSetInfo(ChartSetInfo chartSetInfo, Stream outStream)
        {
            var writer = new StreamWriter(outStream);

            writer.WriteLine("#version " + Version);
            writer.WriteLine();
            writer.WriteLine("#song-metadata");
            writer.WriteLine("title=" + chartSetInfo.SongTitle);
            writer.WriteLine("artist=" + chartSetInfo.SongArtist);
            writer.WriteLine("filename=" + chartSetInfo.SongFileName);

            var charts = chartSetInfo.Charts;
            if (charts.Count > 0)
            {
                foreach (var chart in charts)
                {
                    writer.WriteLine();

                    if (chart.DifficultyIndex != null)
                        writer.WriteLine("#chart " + chart.DifficultyIndex.Value);
                    else writer.WriteLine("#chart");

                    writer.WriteLine("filename=" + chart.FileName);
                    if (chart.DifficultyColor != null)
                    {
                        var color = chart.DifficultyColor.Value;
                        int IVal(float c) => MathL.RoundToInt(MathL.Clamp01(c) * 255);
                        writer.WriteLine($"color={IVal(color.X):X2}{IVal(color.Y):X2}{IVal(color.Z):X2}");
                    }

                    void WriteCheckedString(string prefix, string value)
                    {
                        if (value != null && value.Length != 0 && value != "Unknown")
                            writer.WriteLine(prefix + value);
                    }

                    WriteCheckedString("charter=", chart.Charter);
                    WriteCheckedString("jacket-file=", chart.JacketFileName);
                    WriteCheckedString("jacket-artist=", chart.JacketArtist);
                    WriteCheckedString("background-file=", chart.BackgroundFileName);
                    WriteCheckedString("background-artist=", chart.BackgroundArtist);
                    writer.WriteLine("difficulty-level=" + chart.DifficultyLevel);
                    WriteCheckedString("difficulty-name=", chart.DifficultyName);
                    WriteCheckedString("difficulty-name-short=", chart.DifficultyNameShort);
                    writer.WriteLine("duration=" + chart.ChartDuration.Seconds);
                }
            }

            writer.Flush();
        }
    }
}
