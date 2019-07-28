using System;
using System.IO;

using Newtonsoft.Json;

using theori.Audio.Effects;
using theori.Charting.Effects;

namespace theori.Charting.Serialization
{
    public class ChartWriter : Disposable
    {
        public static ChartWriter ToFile(string filePath)
        {
            return new ChartWriter(new JsonTextWriter(new StreamWriter(File.Open(filePath, FileMode.Create))));
        }

        public static ChartWriter ToString(StringWriter writer)
        {
            return new ChartWriter(new JsonTextWriter(writer));
        }

        private static bool ValueIsDefault(object obj)
        {
            if (obj is ValueType value)
                return value.Equals(Activator.CreateInstance(value.GetType()));
            return obj == null;
        }

        private readonly JsonWriter m_writer;

        private ChartWriter(JsonWriter writer)
        {
            m_writer = writer;
        }

        protected override void DisposeManaged()
        {
            m_writer.Close();
        }

        public void Flush() => m_writer.Flush();

        public void WriteNull() => m_writer.WriteNull();

        public void WriteValue(bool value) => m_writer.WriteValue(value);
        public void WriteValue(sbyte value) => m_writer.WriteValue(value);
        public void WriteValue(short value) => m_writer.WriteValue(value);
        public void WriteValue(int value) => m_writer.WriteValue(value);
        public void WriteValue(long value) => m_writer.WriteValue(value);
        public void WriteValue(byte value) => m_writer.WriteValue(value);
        public void WriteValue(ushort value) => m_writer.WriteValue(value);
        public void WriteValue(uint value) => m_writer.WriteValue(value);
        public void WriteValue(ulong value) => m_writer.WriteValue(value);
        public void WriteValue(float value) => m_writer.WriteValue(value);
        public void WriteValue(double value) => m_writer.WriteValue(value);
        public void WriteValue(string value) => m_writer.WriteValue(value);
        public void WriteValue(char value) => m_writer.WriteValue(value);
        public void WriteValue(tick_t value) => m_writer.WriteValue((double)value);
        public void WriteValue(time_t value) => m_writer.WriteValue((double)value);

        public void WriteValue(object value)
        {
            switch (value)
            {
                //case null: WriteNull(); break;

                case LaneLabel label:
                    switch (label.LabelKind)
                    {
                        case LaneLabel.Kind.Text: WriteValue((string)label); break;
                        case LaneLabel.Kind.Number: WriteValue((int)label); break;
                    }
                    break;

                case Enum enumVal: WriteValue(enumVal.ToString()); break;
                case IEffectParam effectParam: WriteValue(effectParam); break;
                case EffectDef effectDef: WriteValue(effectDef); break;
                case Entity entity: WriteValue(entity); break;

                case bool prim: WriteValue(prim); break;
                case sbyte prim: WriteValue(prim); break;
                case short prim: WriteValue(prim); break;
                case int prim: WriteValue(prim); break;
                case long prim: WriteValue(prim); break;
                case byte prim: WriteValue(prim); break;
                case ushort prim: WriteValue(prim); break;
                case uint prim: WriteValue(prim); break;
                case ulong prim: WriteValue(prim); break;
                case float prim: WriteValue(prim); break;
                case double prim: WriteValue(prim); break;
                case string prim: WriteValue(prim); break;
                case char prim: WriteValue(prim); break;
                case tick_t prim: WriteValue(prim); break;
                case time_t prim: WriteValue(prim); break;

                case object obj: WriteFromReflection(obj); break;

                default: throw new ArgumentException($"Unable to write object `{ value }` to chart.");
            }
        }

        public void WriteValue(IEffectParam effectParam)
        {
            switch (effectParam)
            {
                case EffectParamX xp:
                {
                    if (xp.IsRange)
                    {
                        WriteStartStructure();

                        WritePropertyName("type");
                        WriteValue("x");

                        WritePropertyName("min");
                        WriteValue(xp.MinValueReal);

                        WritePropertyName("max");
                        WriteValue(xp.MaxValueReal);

                        WriteEndStructure();
                    }
                    else WriteValue(xp.MinValue);
                }
                break;

                case EffectParamI ip:
                {
                    if (ip.IsRange)
                    {
                        WriteStartStructure();

                        WritePropertyName("ease");
                        WriteValue(ip.Ease);

                        WritePropertyName("min");
                        WriteValue(ip.MinValue);

                        WritePropertyName("max");
                        WriteValue(ip.MaxValue);

                        WriteEndStructure();
                    }
                    else WriteValue(ip.MinValue);
                }
                break;

                case EffectParamF fp:
                {
                    if (fp.IsRange)
                    {
                        WriteStartStructure();

                        WritePropertyName("ease");
                        WriteValue(fp.Ease);

                        WritePropertyName("min");
                        WriteValue(fp.MinValue);

                        WritePropertyName("max");
                        WriteValue(fp.MaxValue);

                        WriteEndStructure();
                    }
                    else WriteValue(fp.MinValue);
                }
                break;

                case EffectParamS sp: WriteValue(sp.MinValue); break;
            }
        }

        public void WriteValue(EffectDef effectDef)
        {
            WriteStartStructure();

            WritePropertyName("type");
            WriteValue(EffectDef.GetEffectIdByType(effectDef.GetType()));

            WritePropertiesFromReflection(effectDef);

            WriteEndStructure();
        }

        public void WriteValue(Entity entity)
        {
            WriteStartStructure();

            WritePropertyName("type");
            WriteValue(Entity.GetEntityIdByType(entity.GetType()));

            WritePropertiesFromReflection(entity);

            WriteEndStructure();
        }

        public void WritePropertyName(string name) => m_writer.WritePropertyName(name);

        public void WriteStartStructure() => m_writer.WriteStartObject();
        public void WriteEndStructure() => m_writer.WriteEndObject();

        public void WriteStartArray() => m_writer.WriteStartArray();
        public void WriteEndArray() => m_writer.WriteEndArray();

        public void WriteInStructure(Action<ChartWriter> action)
        {
            WriteStartStructure();
            action?.Invoke(this);
            WriteEndStructure();
        }

        public void WriteInArray(Action<ChartWriter> action)
        {
            WriteStartArray();
            action?.Invoke(this);
            WriteEndArray();
        }

        private void WriteFromReflection(object obj)
        {
            WriteStartStructure();
            WritePropertiesFromReflection(obj);
            WriteEndStructure();
        }

        private void WritePropertiesFromReflection(object obj)
        {
            foreach (var prop in obj.GetTheoriPropertyInfos())
            {
                object value = prop.Value;
                if (prop.HasAttribute<TheoriIgnoreDefaultAttribute>() && ValueIsDefault(value)) continue;

                WritePropertyName(prop.GetAttribute<TheoriPropertyAttribute>()?.OverrideName ?? prop.Name);
                WriteValue(value);
            }
        }
    }
}
