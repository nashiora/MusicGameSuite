using System;
using System.Collections.Generic;
using System.IO;

using theori.GameModes;

namespace theori.Charting.IO
{
    public abstract class ChartObjectSerializer
    {
        /// <summary>
        /// A value > 0
        /// </summary>
        public abstract int ID { get; }

        public abstract string SerializeSubclass(ChartObject obj);
    }

    public abstract class ChartObjectSerializer<T> : ChartObjectSerializer
        where T : ChartObject
    {
        public sealed override string SerializeSubclass(ChartObject obj) => SerializeSubclass(obj as T);
        public abstract string SerializeSubclass(T obj);
    }

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

        private readonly Dictionary<Type, ChartObjectSerializer> m_serializersByType = new Dictionary<Type, ChartObjectSerializer>();
        private readonly Dictionary<int, ChartObjectSerializer> m_serializersByID = new Dictionary<int, ChartObjectSerializer>();

        private ChartSerializer(GameMode mode = null)
        {
            m_mode = mode;
        }

        private ChartObjectSerializer GetSerializerForType(ChartObject obj)
        {
            if (!m_serializersByType.TryGetValue(obj.GetType(), out var serializer))
            {
                serializer = m_mode.GetSerializerFor(obj);
                m_serializersByType[obj.GetType()] = serializer;
            }
            return serializer;
        }

        private ChartObjectSerializer GetSerializerByID(int id)
        {
            if (!m_serializersByID.TryGetValue(id, out var serializer))
            {
                serializer = m_mode.GetSerializerByID(id);
                m_serializersByID[id] = serializer;
            }
            return serializer;
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

                foreach (var obj in stream)
                {
                    var serializer = GetSerializerForType(obj);
                    if (serializer != null)
                        writer.WriteLine($"{serializer.ID},{(double)obj.Position},{(double)obj.Duration} {serializer.SerializeSubclass(obj)}");
                    else writer.WriteLine($"0,{(double)obj.Position},{(double)obj.Duration}");
                }
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
