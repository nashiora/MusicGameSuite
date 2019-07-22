using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using theori.Audio.Effects;
using theori.GameModes;

namespace theori.Charting.IO
{
    // TODO(local): figure out how to properly handle versioning
    public class BinaryTheoriChartSerializer : IChartSerializer
    {
        public const uint MAGIC = 0xFEEDF00D;
        public const byte VERSION = 1;

        public static BinaryTheoriChartSerializer GetDefaultSerializer()
        {
            return new BinaryTheoriChartSerializer();
        }

        public static BinaryTheoriChartSerializer GetSerializerFor(GameMode gameMode)
        {
            return new BinaryTheoriChartSerializer(gameMode);
        }

        public static BinaryTheoriChartSerializer GetSerializerForShared<T>()
            where T : GameMode
        {
            //return Host.GetSharedGameMode<T>().CreateChartSerializer() ?? GetDefaultSerializer();
            return null;
        }

        public string FormatExtension => ".theori";

        private readonly GameMode m_mode;

        private readonly Dictionary<Type, ChartObjectSerializer> m_serializersByType = new Dictionary<Type, ChartObjectSerializer>();
        private readonly Dictionary<int, ChartObjectSerializer> m_serializersByID = new Dictionary<int, ChartObjectSerializer>();

        public BinaryTheoriChartSerializer(GameMode mode = null)
        {
            m_mode = mode;
        }

        private ChartObjectSerializer GetSerializerForType(ChartObject obj)
        {
            if (m_mode == null) return null;
            if (!m_serializersByType.TryGetValue(obj.GetType(), out var serializer))
            {
                serializer = m_mode.GetSerializerFor(obj);
                m_serializersByType[obj.GetType()] = serializer;
            }
            return serializer;
        }

        private ChartObjectSerializer GetSerializerByID(int id)
        {
            if (m_mode == null) return null;
            if (!m_serializersByID.TryGetValue(id, out var serializer))
            {
                serializer = m_mode.GetSerializerByID(id);
                m_serializersByID[id] = serializer;
            }
            return serializer;
        }

        public EffectDef DeserializeEffectDef(BinaryReader reader)
        {
            byte typeId = reader.ReadUInt8();
            var type = (EffectType)typeId;

            Debug.Assert(type != EffectType.None);

            var mix = ReadValuesF();
            switch (type)
            {
                case EffectType.Retrigger:
                {
                    var gating = ReadValuesF();
                    var gateDuration = ReadValuesF();
                    return new RetriggerEffectDef(mix, gating, gateDuration);
                }

                case EffectType.Flanger:
                {
                    var delay = ReadValuesF();
                    var offset = ReadValuesI();
                    var depth = ReadValuesI();
                    return new FlangerEffectDef(mix, delay, offset, depth);
                }

                case EffectType.Phaser: return new PhaserEffectDef(mix);

                case EffectType.Gate:
                {
                    var gating = ReadValuesF();
                    var gateDuration = ReadValuesF();
                    return new GateEffectDef(mix, gating, gateDuration);
                }

                case EffectType.TapeStop: return new TapeStopEffectDef(mix, ReadValuesF());
                case EffectType.BitCrush: return new BitCrusherEffectDef(mix, ReadValuesI());
                case EffectType.Wobble: return new WobbleEffectDef(mix, ReadValuesF());

                case EffectType.SideChain:
                {
                    var amount = ReadValuesF();
                    var duration = ReadValuesF();
                    return new SideChainEffectDef(mix, amount, duration);
                }

                case EffectType.LowPassFilter:
                case EffectType.HighPassFilter:
                case EffectType.PeakingFilter:
                {
                    var q = ReadValuesF();
                    var gain = ReadValuesF();
                    var freq = ReadValuesF();
                    return new BiQuadFilterEffectDef(type, mix, q, gain, freq);
                }

                default: throw new ChartFormatException($"Invalid effect ID { typeId }.");
            }

            EffectParamF ReadValuesF()
            {
                bool single = reader.ReadUInt8() == 0;
                float min = reader.ReadSingleBE();
                if (single) return min;
                float max = reader.ReadSingleBE();
                return new EffectParamF(min, max);
            }

            EffectParamI ReadValuesI()
            {
                bool single = reader.ReadUInt8() == 0;
                int min = reader.ReadInt32BE();
                if (single) return min;
                int max = reader.ReadInt32BE();
                return new EffectParamI(min, max);
            }
        }

        public void SerializeEffectDef(EffectDef effectDef, BinaryWriter writer)
        {
            var type = effectDef.Type;
            byte typeId = (byte)type;

            writer.WriteUInt8(typeId);
            WriteValuesF(effectDef.Mix);

            switch (effectDef)
            {
                case RetriggerEffectDef r: Debug.Assert(type == EffectType.Retrigger);
                {
                    WriteValuesF(r.Gating);
                    WriteValuesF(r.GateDuration);
                } break;

                case FlangerEffectDef l: Debug.Assert(type == EffectType.Flanger);
                {
                    WriteValuesF(l.Delay);
                    WriteValuesI(l.Offset);
                    WriteValuesI(l.Depth);
                } break;

                case PhaserEffectDef ph: Debug.Assert(type == EffectType.Phaser);
                {
                } break;
                    
                case GateEffectDef g: Debug.Assert(type == EffectType.Gate);
                {
                    WriteValuesF(g.Gating);
                    WriteValuesF(g.GateDuration);
                } break;
                    
                case TapeStopEffectDef ts: Debug.Assert(type == EffectType.TapeStop);
                {
                    WriteValuesF(ts.Duration);
                } break;
                    
                case BitCrusherEffectDef bc: Debug.Assert(type == EffectType.BitCrush);
                {
                    WriteValuesI(bc.Reduction);
                } break;
                    
                case WobbleEffectDef w: Debug.Assert(type == EffectType.Wobble);
                {
                    WriteValuesF(w.Period);
                } break;
                    
                case SideChainEffectDef s: Debug.Assert(type == EffectType.SideChain);
                {
                    WriteValuesF(s.Amount);
                    WriteValuesF(s.Duration);
                } break;
                    
                case BiQuadFilterEffectDef bq:
                {
                    WriteValuesF(bq.Q);
                    WriteValuesF(bq.Gain);
                    WriteValuesF(bq.Freq);
                } break;
            }

            void WriteValuesF(EffectParamF p)
            {
                writer.WriteUInt8((byte)(p.IsRange ? 0xFF : 0));
                writer.WriteSingleBE(p.MinValue);
                if (p.IsRange) writer.WriteSingleBE(p.MaxValue);
            }

            void WriteValuesI(EffectParamI p)
            {
                writer.WriteUInt8((byte)(p.IsRange ? 0xFF : 0));
                writer.WriteInt32BE(p.MinValue);
                if (p.IsRange) writer.WriteInt32BE(p.MaxValue);
            }
        }

        public Chart LoadFromFile(string parentDirectory, ChartInfo chartInfo)
        {
            string filePath = Path.Combine(parentDirectory, chartInfo.Set.FilePath, chartInfo.FileName);
            using (var reader = new BinaryReader(File.OpenRead(filePath), Encoding.UTF8))
                return DeserializeChart(chartInfo, reader);
        }

        private Chart DeserializeChart(ChartInfo chartInfo, BinaryReader reader)
        {
            uint magicCheck = reader.ReadUInt32BE();
            if (magicCheck != MAGIC)
                throw new ChartFormatException($"Invalid input stream given.");

            uint versionCheck = reader.ReadUInt8();
            if (versionCheck > VERSION)
                throw new ChartFormatException($"Input stream cannot be read by this serializer: the version is too high.");

            ushort streamCount = reader.ReadUInt16BE();
            var chart = new Chart(streamCount) { Info = chartInfo };

            chart.Offset = chartInfo.ChartOffset;

            int effectCount = reader.ReadUInt16BE();
            var effectTable = new ChartEffectTable();

            for (int i = 0; i < effectCount; i++)
            {
                var effect = DeserializeEffectDef(reader);
                effectTable.Add(effect);
            }

            ushort controlPointCount = reader.ReadUInt16BE();
            for (int i = 0; i < controlPointCount; i++)
            {
                tick_t position = reader.ReadDoubleBE();
                double bpm = reader.ReadDoubleBE();
                int beatCount = reader.ReadUInt8();
                int beatKind = reader.ReadUInt8();
                double mult = reader.ReadDoubleBE();

                var cp = chart.ControlPoints.GetOrCreate(position, false);
                cp.BeatsPerMinute = bpm;
                cp.BeatCount = beatCount;
                cp.BeatKind = beatKind;
                cp.SpeedMultiplier = mult;
            }

            for (int s = 0; s < streamCount; s++)
            {
                var stream = chart[s];

                uint ucount = reader.ReadUInt32BE();
                if (ucount > int.MaxValue)
                    throw new ChartFormatException($"Too many objects declared in stream { s }.");
                int count = (int)ucount;

                for (int i = 0; i < count; i++)
                {
                    byte objId = reader.ReadUInt8();
                    var serializer = GetSerializerByID(objId);

                    byte flags = reader.ReadUInt8();
                    bool hasDuration = (flags & 0x01) != 0;

                    tick_t position = reader.ReadDoubleBE();
                    tick_t duration = hasDuration ? reader.ReadDoubleBE() : 0;

                    ChartObject obj;
                    if (serializer != null)
                        obj = serializer.DeserializeSubclass(position, duration, reader, effectTable);
                    else obj = new ChartObject() { Position = position, Duration = duration, };

                    stream.Add(obj);
                }
            }

            return chart;
        }

        public void SaveToFile(string parentDirectory, Chart chart)
        {
            string filePath = Path.Combine(parentDirectory, chart.Info.Set.FilePath, chart.Info.FileName);
            using (var writer = new BinaryWriter(File.Open(filePath, FileMode.Create), Encoding.UTF8))
                SerializeChart(chart, writer);
        }

        private void SerializeChart(Chart chart, BinaryWriter writer)
        {
            writer.WriteUInt32BE(MAGIC);
            writer.WriteUInt8(VERSION);

            writer.WriteUInt16BE((ushort)chart.StreamCount);
            Logger.Log($"chart.binwrite stream count { chart.StreamCount }");

            var effectTable = new ChartEffectTable();
            for (int s = 0; s < chart.StreamCount; s++)
            {
                var stream = chart[s];
                foreach (var obj in stream)
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

            writer.WriteUInt16BE((ushort)effectTable.Count);
            Logger.Log($"chart.binwrite effect count { effectTable.Count }");

            for (int i = 0; i < effectTable.Count; i++)
            {
                var effect = effectTable[i];
                Logger.Log($"chart.binwrite   effect { effect.Type }");
                SerializeEffectDef(effect, writer);
            }

            int controlPointCount = chart.ControlPoints.Count;
            writer.WriteUInt16BE((ushort)controlPointCount);
            Logger.Log($"chart.binwrite control point count { controlPointCount }");

            for (int i = 0; i < controlPointCount; i++)
            {
                var cp = chart.ControlPoints[i];
                Logger.Log($"chart.binwrite   control point { cp.Position } { cp.BeatsPerMinute } { cp.BeatCount }/{ cp.BeatKind } { cp.SpeedMultiplier }");

                writer.WriteDoubleBE((double)cp.Position);
                writer.WriteDoubleBE(cp.BeatsPerMinute);
                writer.WriteUInt8((byte)cp.BeatCount);
                writer.WriteUInt8((byte)cp.BeatKind);
                writer.WriteDoubleBE(cp.SpeedMultiplier);
            }

            for (int s = 0; s < chart.StreamCount; s++)
            {
                var stream = chart[s];

                int count = stream.Count;
                writer.WriteUInt32BE((uint)count);

                for (int i = 0; i < count; i++)
                {
                    var obj = stream[i];

                    var serializer = GetSerializerForType(obj);
                    byte objId = (byte)(serializer?.ID ?? 0);

                    writer.WriteUInt8(objId);
                    if (obj.IsInstant)
                        writer.WriteUInt8(0);
                    else writer.WriteUInt8(0x01);
                    writer.WriteDoubleBE((double)obj.Position);
                    if (!obj.IsInstant)
                        writer.WriteDoubleBE((double)obj.Duration);

                    serializer?.SerializeSubclass(obj, writer, effectTable);
                }
            }
        }
    }
}
