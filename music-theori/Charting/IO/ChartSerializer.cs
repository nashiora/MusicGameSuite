using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using theori.Audio.Effects;
using theori.GameModes;

namespace theori.Charting.IO
{
    public sealed class ChartEffectTable
    {
        private readonly List<EffectDef> m_effects = new List<EffectDef>();

        public EffectDef this[int index] => m_effects[index];
        public int Count => m_effects.Count;

        internal ChartEffectTable()
        {
        }

        public int IndexOf(EffectDef effect) => m_effects.IndexOf(effect);

        public int Add(EffectDef effect)
        {
            int index = m_effects.IndexOf(effect);
            if (index < 0)
            {
                index = m_effects.Count;
                m_effects.Add(effect);
            }
            return index;
        }
    }

    public abstract class ChartObjectSerializer
    {
        /// <summary>
        /// A locally unique value > 0
        /// </summary>
        public readonly int ID;

        protected ChartObjectSerializer(int id)
        {
            ID = id;
        }

        public abstract void SerializeSubclass(Entity obj, BinaryWriter writer, ChartEffectTable effects);

        public abstract Entity DeserializeSubclass(tick_t pos, tick_t dur, BinaryReader reader, ChartEffectTable effects);
    }

    public abstract class ChartObjectSerializer<T> : ChartObjectSerializer
        where T : Entity
    {
        protected ChartObjectSerializer(int id) : base(id) { }

        public sealed override void SerializeSubclass(Entity obj, BinaryWriter writer, ChartEffectTable effects) => SerializeSubclass(obj as T, writer, effects);
        public abstract void SerializeSubclass(T obj, BinaryWriter writer, ChartEffectTable effects);
    }

    public class ChartSerializer
    {
        public readonly string ChartsDir;

        private readonly GameMode m_gameMode;

        public ChartSerializer(string chartsDir, GameMode gameMode = null)
        {
            ChartsDir = chartsDir;
            m_gameMode = gameMode;
        }

        public ChartSetInfo LoadSetFromFile()
        {
            return null;
        }

        public Chart LoadChartFromFile(ChartInfo chartInfo)
        {
            string chartFile = Path.Combine(ChartsDir, chartInfo.Set.FilePath, chartInfo.FileName);

            Chart chart = null;
            using (var reader = new JsonTextReader(new StreamReader(File.OpenRead(chartFile))))
            {
            }

            return chart;
        }

        public void SaveChartToFile(Chart chart)
        {
            var chartInfo = chart.Info;
            string chartFile = Path.Combine(ChartsDir, chartInfo.Set.FilePath, chartInfo.FileName);

            var effectTable = new ChartEffectTable();
            foreach (var lane in chart.Lanes)
            {
                foreach (var obj in lane)
                {
                    if (obj is IHasEffectDef e)
                    {
                        var effect = e.Effect;
                        if (effect == null || effect.Type == EffectType.None)
                            continue;
                        effectTable.Add(effect);
                    }
                }
            }

            var stringWriter = new StringWriter();
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.WriteStartObject();
                {
                    writer.WritePropertyName("Effects");
                    writer.WriteStartArray();
                    {
                        for (int i = 0; i < effectTable.Count; i++)
                            WriteValue(effectTable[i]);
                    }
                    writer.WriteEndArray();

                    writer.WritePropertyName("ControlPoints");
                    writer.WriteStartArray();
                    {
                        for (int i = 0; i < chart.ControlPoints.Count; i++)
                        {
                            var cp = chart.ControlPoints[i];
                            if (cp == null)
                                Logger.Log($"Null object in control points at { i }");
                            else WriteValue(cp);
                        }
                    }
                    writer.WriteEndArray();

                    writer.WritePropertyName("Objects");
                    writer.WriteStartArray();
                    {
                        foreach (var lane in chart.Lanes)
                        {
                            writer.WriteStartArray();
                            for (int i = 0; i < lane.Count; i++)
                            {
                                var obj = lane[i];
                                if (obj == null)
                                    Logger.Log($"Null object in stream { lane.Label } at { i }");
                                else WriteValue(obj);
                            }
                            writer.WriteEndArray();
                        }
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndObject();

                writer.Flush();
                string result = stringWriter.ToString();

                File.WriteAllText(chartFile, FormatJson(result));

                void WriteValue(object obj)
                {
                    if (obj == null)
                    {
                        writer.WriteNull();
                        return;
                    }

                    var objType = obj.GetType();
                    if (obj is Enum e)
                        writer.WriteValue(e.ToString());
                    else if (obj is string str)
                        writer.WriteValue(str);
                    else if (obj is tick_t tick)
                        writer.WriteValue((double)tick);
                    else if (obj is time_t time)
                        writer.WriteValue((double)time);
                    else if (objType.IsPrimitive)
                        writer.WriteValue(obj);
                    else if (obj is IEffectParam ep)
                    {
                        switch (ep)
                        {
                            case EffectParamX xp:
                            {
                                if (xp.IsRange)
                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName("MinValue");
                                    writer.WriteValue(xp.MinValueReal);

                                    writer.WritePropertyName("MaxValue");
                                    writer.WriteValue(xp.MaxValueReal);

                                    writer.WriteEndObject();
                                }
                                else writer.WriteValue(xp.MinValueReal);
                            } break;

                            case EffectParamI ip:
                            {
                                if (ip.IsRange)
                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName("MinValue");
                                    writer.WriteValue(ip.MinValue);

                                    writer.WritePropertyName("MaxValue");
                                    writer.WriteValue(ip.MaxValue);

                                    writer.WriteEndObject();
                                }
                                else writer.WriteValue(ip.MinValue);
                            } break;

                            case EffectParamF fp:
                            {
                                if (fp.IsRange)
                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName("MinValue");
                                    writer.WriteValue(fp.MinValue);

                                    writer.WritePropertyName("MaxValue");
                                    writer.WriteValue(fp.MaxValue);

                                    writer.WriteEndObject();
                                }
                                else writer.WriteValue(fp.MinValue);
                            } break;

                            case EffectParamS sp: writer.WriteValue(sp.MinValue); break;
                        }
                    }
                    else
                    {
                        writer.WriteStartObject();
                        {
                            // NOTE(local): Currently this system assumes all type information can be gathered EXCEPT that of ChartObjects (and EffectDefs)
                            if (obj is Entity cobj)
                            {
                                writer.WritePropertyName("ChartObjectType");
                                var id = Entity.GetEntityIdByType(objType);
                                writer.WriteValue(id);
                            }
                            else if (obj is EffectDef effect)
                            {
                                writer.WritePropertyName("EffectType");
                                string name = objType.Name;
                                if (name.EndsWith(nameof(EffectDef)))
                                    name = name.Substring(0, name.IndexOf(nameof(EffectDef)));
                                // NOTE(local): use similar naming convention to chart objects in case of allowing game mode created effects
                                writer.WriteValue($"theori.{ name }");
                            }

                            //var flags = BindingFlags.Instance | BindingFlags.Public;
                            var fields = from type in objType.GetFields()
                                         where type.GetCustomAttribute<EntityIgnoreAttribute>() == null &&
                                              (type.GetCustomAttribute<EntityPropertyAttribute>() != null || type.IsPublic)
                                         select type;
                            var props = from prop in objType.GetProperties()
                                        where prop.SetMethod != null && prop.GetMethod != null
                                        where prop.GetCustomAttribute<EntityIgnoreAttribute>() == null && (prop.GetCustomAttribute<EntityPropertyAttribute>() != null ||
                                             (prop.SetMethod.IsPublic && prop.SetMethod.GetCustomAttribute<EntityIgnoreAttribute>() == null &&
                                              prop.GetMethod.IsPublic && prop.GetMethod.GetCustomAttribute<EntityIgnoreAttribute>() == null))
                                        select prop;

                            foreach (var field in fields)
                            {
                                object value = field.GetValue(obj);
                                if (field.GetCustomAttribute<EntityIgnoreDefaultAttribute>() != null && ValueIsDefault(value)) continue;

                                writer.WritePropertyName(field.Name);
                                WriteValue(value);
                            }

                            foreach (var prop in props)
                            {
                                object value = prop.GetValue(obj);
                                // TODO(local): for the get/set pairs too?
                                if (prop.GetCustomAttribute<EntityIgnoreDefaultAttribute>() != null && ValueIsDefault(value)) continue;

                                writer.WritePropertyName(prop.Name);
                                WriteValue(value);
                            }
                        }
                        writer.WriteEndObject();
                    }
                }
            }
        }

        private bool ValueIsDefault(object obj)
        {
            if (obj is ValueType value)
                return value.Equals(Activator.CreateInstance(value.GetType()));
            return obj == null;
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
