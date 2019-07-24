using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using theori.GameModes;

namespace theori.Charting.Serialization
{
    public sealed class ChartSerializer
    {
        public string ParentDirectory { get; }

        private readonly GameMode m_gameMode;

        public ChartSerializer(string chartsDir, GameMode gameMode = null)
        {
            ParentDirectory = chartsDir;
            m_gameMode = gameMode;
        }

        public Chart LoadFromFile(ChartInfo chartInfo)
        {
            throw new NotImplementedException();

            string chartFile = Path.Combine(ParentDirectory, chartInfo.Set.FilePath, chartInfo.FileName);

            Chart chart = null;
            using (var reader = new JsonTextReader(new StreamReader(File.OpenRead(chartFile))))
            {
            }

            return chart;
        }

        public void SaveToFile(Chart chart)
        {
            var chartInfo = chart.Info;
            string chartFile = Path.Combine(ParentDirectory, chartInfo.Set.FilePath, chartInfo.FileName);

            var stringWriter = new StringWriter();
            using (var writer = ChartWriter.ToString(stringWriter))
            {
                writer.WriteStartStructure();
                {
                    writer.WritePropertyName("control-points");
                    writer.WriteStartArray();
                    {
                        for (int i = 0; i < chart.ControlPoints.Count; i++)
                        {
                            var cp = chart.ControlPoints[i];
                            if (cp == null)
                                Logger.Log($"Null object in control points at { i }");
                            else writer.WriteValue(cp);
                        }
                    }
                    writer.WriteEndArray();

                    writer.WritePropertyName("lanes");
                    writer.WriteStartArray();
                    {
                        foreach (var lane in chart.Lanes)
                        {
                            writer.WriteStartStructure();

                            writer.WritePropertyName("label");
                            writer.WriteValue(lane.Label);

                            writer.WritePropertyName("entities");
                            writer.WriteStartArray();
                            for (int i = 0; i < lane.Count; i++)
                            {
                                var obj = lane[i];
                                if (obj == null)
                                    Logger.Log($"Null object in stream { lane.Label } at { i }");
                                else writer.WriteValue(obj);
                            }
                            writer.WriteEndArray();

                            writer.WriteEndStructure();
                        }
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndStructure();

                writer.Flush();
                string result = stringWriter.ToString();

                File.WriteAllText(chartFile, FormatJson(result));
            }
        }

        private string FormatJson(string json)
        {
            const string INDENT_STRING = "    ";
            int indentation = 0;
            int quoteCount = 0;
            var result =
                from ch in json
                let quotes = ch == '"' ? quoteCount++ : quoteCount
                let lineBreak = ch == ',' && quotes % 2 == 0 ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(INDENT_STRING, indentation)) : null
                let openChar = ch == '{' || ch == '[' ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(INDENT_STRING, ++indentation)) : ch.ToString()
                let closeChar = ch == '}' || ch == ']' ? Environment.NewLine + string.Concat(Enumerable.Repeat(INDENT_STRING, --indentation)) + ch : ch.ToString()
                select lineBreak ?? (openChar.Length > 1 ? openChar : closeChar);

            return string.Concat(result);
        }
    }
}
