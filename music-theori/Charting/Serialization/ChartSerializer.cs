using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using theori.Audio.Effects;
using theori.Charting.Effects;
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
            string chartFile = Path.Combine(ParentDirectory, chartInfo.Set.FilePath, chartInfo.FileName);

            // TODO(local): We don't need to be creating chart factories like this aaa but it's a start
            Chart chart = m_gameMode.CreateChartFactory().CreateNew();
            chart.Info = chartInfo;

            var jobj = JObject.Load(new JsonTextReader(new StreamReader(File.OpenRead(chartFile))));
            dynamic jobjdyn = jobj;

            JArray controlPoints = jobjdyn.controlPoints;
            JArray lanes = jobjdyn.lanes;

            LaneLabel ToLabel(JToken token)
            {
                switch (token.Type)
                {
                    case JTokenType.String: return (string)token;
                    case JTokenType.Integer: return (int)token;
                    default: throw new ChartFormatException("Invalid value for lane label.");
                }
            }

            object ToValue(dynamic jObjectDyn, Type typeHint)
            {
                // TODO(local): Check that any of these are actually okay???
                if (typeHint == typeof(bool)) return (bool)jObjectDyn;
                else if (typeHint == typeof(sbyte)) return (sbyte)jObjectDyn;
                else if (typeHint == typeof(short)) return (short)jObjectDyn;
                else if (typeHint == typeof(int)) return (int)jObjectDyn;
                else if (typeHint == typeof(long)) return (long)jObjectDyn;
                else if (typeHint == typeof(byte)) return (byte)jObjectDyn;
                else if (typeHint == typeof(ushort)) return (ushort)jObjectDyn;
                else if (typeHint == typeof(uint)) return (uint)jObjectDyn;
                else if (typeHint == typeof(ulong)) return (ulong)jObjectDyn;
                else if (typeHint == typeof(float)) return (float)jObjectDyn;
                else if (typeHint == typeof(double)) return (double)jObjectDyn;
                else if (typeHint == typeof(string)) return (string)jObjectDyn;
                else if (typeHint == typeof(char)) return (char)jObjectDyn;
                else if (typeHint == typeof(tick_t)) return (tick_t)(double)jObjectDyn;
                else if (typeHint == typeof(time_t)) return (time_t)(double)jObjectDyn;
                else if (typeHint.IsEnum) return Enum.Parse(typeHint, (string)jObjectDyn);
                else if (typeof(IEffectParam).IsAssignableFrom(typeHint))
                {
                    bool isRange = jObjectDyn is JObject;
                    if (typeHint == typeof(EffectParamI))
                    {
                        if (isRange)
                            return new EffectParamI((int)jObjectDyn.min, (int)jObjectDyn.max, (Ease)Enum.Parse(typeof(Ease), (string)jObjectDyn.ease));
                        return new EffectParamI((int)jObjectDyn);
                    }
                    else if (typeHint == typeof(EffectParamF))
                    {
                        if (isRange)
                        {
                            if (((JObject)jObjectDyn).ContainsKey("type") && (string)jObjectDyn.type == "x")
                                return new EffectParamX((int)jObjectDyn.min, (int)jObjectDyn.max);
                            return new EffectParamF((float)jObjectDyn.min, (float)jObjectDyn.max, (Ease)Enum.Parse(typeof(Ease), (string)jObjectDyn.ease));
                        }
                        return new EffectParamF((float)jObjectDyn);
                    }
                    else if (typeHint == typeof(EffectParamX))
                    {
                        if (isRange)
                            return new EffectParamX((int)jObjectDyn.min, (int)jObjectDyn.max);
                        return new EffectParamX((int)jObjectDyn);
                    }
                    else if (typeHint == typeof(EffectParamS))
                        return new EffectParamS((string)jObjectDyn);
                    return null;
                }
                else
                {
                    var obj = Activator.CreateInstance(typeHint);
                    var jObject = (JObject)jObjectDyn;

                    foreach (var prop in obj.GetTheoriPropertyInfos())
                    {
                        string name = prop.GetAttribute<TheoriPropertyAttribute>()?.OverrideName ?? prop.Name;
                        if (!jObject.ContainsKey(name)) continue;

                        if (prop.Type == typeof(EffectDef) || prop.Type.IsSubclassOf(typeof(EffectDef)))
                            prop.Value = ToEffectDef(jObject.GetValue(name));
                        else prop.Value = ToValue(jObject.GetValue(name), prop.Type);
                    }

                    return obj;
                }

            }

            Entity ToEntity(dynamic entityObj)
            {
                string entityId = (string)entityObj.type;
                return (Entity)ToValue(entityObj, Entity.GetEntityTypeById(entityId));
            }

            EffectDef ToEffectDef(dynamic effectObj)
            {
                string effectId = (string)effectObj.type;
                return (EffectDef)ToValue(effectObj, EffectDef.GetEntityTypeById(effectId));
            }

            tick_t ToTickT(JToken token) => (double)token;

            foreach (dynamic pointObj in controlPoints)
            {
                tick_t pos = ToTickT(pointObj.position);
                var point = chart.ControlPoints.GetOrCreate(pos, true);
                point.BeatCount = (int)pointObj.numerator;
                point.BeatKind = (int)pointObj.denominator;
                point.SpeedMultiplier = (double)pointObj.multiplier;
                point.BeatsPerMinute = (double)pointObj.bpm;
            }

            foreach (dynamic laneObj in lanes)
            {
                var lane = chart[ToLabel(laneObj.label)];

                foreach (dynamic entityObj in laneObj.entities)
                {
                    var entity = ToEntity(entityObj);
                    lane.Add(entity);
                }
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
                    writer.WritePropertyName("controlPoints");
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
