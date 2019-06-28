using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using theori.Audio.Effects;

namespace NeuroSonic.Charting.KShootMania
{
    public class Block
    {
        public readonly List<Tick> Ticks = new List<Tick>();

        public int TickCount => Ticks.Count;
        public Tick this[int index] => Ticks[index];
    }

    public class Tick
    {
        public readonly List<TickSetting> Settings = new List<TickSetting>();
        
        public readonly ButtonData[] Bt = new ButtonData[4];
        public readonly ButtonData[] Fx = new ButtonData[2];
        public readonly LaserData[] Laser = new LaserData[2];

        public AddData Add;
    }

    public struct TickSetting
    {
        public string Key;
        public Variant Value;

        public TickSetting(string key, Variant value)
        {
            Key = key;
            Value = value;
        }
    }

    public enum ButtonState
    {
        Off, Chip, Hold, ChipSample,
    }
    
    public enum LaserState
    {
        Inactive, Lerp, Position,
    }

    public struct ButtonData
    {
        public ButtonState State;
        public FxKind FxKind;
    }

    public enum FxKind
    {
        None = 0,

        BitCrush = 'B',
        
        Gate4  = 'G',
        Gate8  = 'H',
        Gate16 = 'I',
        Gate32 = 'J',
        Gate12 = 'K',
        Gate24 = 'L',
        
        Retrigger8  = 'S',
        Retrigger16 = 'T',
        Retrigger32 = 'U',
        Retrigger12 = 'V',
        Retrigger24 = 'W',

        Phaser = 'Q',
        Flanger = 'F',
        Wobble = 'X',
        SideChain = 'D',
        TapeStop = 'A',
    }

    public struct LaserData
    {
        public LaserState State;
        public LaserPosition Position;
    }

    public struct LaserPosition
    {
        public const int Resolution = 51;

        class Chars : Dictionary<char, int>
        {
            public int NumChars;

            public Chars()
            {
                void AddRange(char start, char end)
                {
                    for (char c = start; c <= end; c++)
                        this[c] = NumChars++;
                }

			    AddRange('0', '9');
			    AddRange('A', 'Z');
			    AddRange('a', 'o');

                Debug.Assert(NumChars == Resolution);
            }
        }

        static Chars chars = new Chars();

        public float Alpha
        {
            get => value / (float)(Resolution - 1);
            set => Value = (int)Math.Round(value * (Resolution - 1));
        }

        private int value;
        public int Value
        {
            get => value;
            set => this.value = MathL.Clamp(value, 0, chars.NumChars - 1);
        }

        public char Image
        {
            get
            {
                int v = Value;
                return chars.Where(kvp => kvp.Value == v).Single().Key;
            }

            set => chars.TryGetValue(value, out this.value);
        }

        public LaserPosition(int value)
        {
            this.value = MathL.Clamp(value, 0, chars.NumChars - 1);
        }

        public LaserPosition(char image)
        {
            chars.TryGetValue(image, out value);
        }
    }

    public enum AddKind
    {
        None, Spin, Swing, Wobble
    }

    public struct AddData
    {
        public AddKind Kind;
        public int Direction;
        public int Duration;
        public int Amplitude;
        public int Frequency;
        public int Decay;
    }

    public struct TickRef
    {
        public int Block, Index, MaxIndex;
        public Tick Tick;
    }

    /// <summary>
    /// Contains all relevant data for a single chart.
    /// </summary>
    public sealed class Chart : IEnumerable<TickRef>
    {
        internal const string SEP = "--";

        public static Chart CreateFromFile(string fileName)
        {
            using (var reader = File.OpenText(fileName))
                return Create(reader);
        }

        public static Chart Create(StreamReader reader)
        {
            var chart = new Chart
            {
                Metadata = ChartMetadata.Create(reader)
            };
            
            void TryAddBuiltInFx(string effect, float bpm)
            {
                string effectName = effect;
                if (effect.TrySplit(';', out string _name, out string paramList))
                    effectName = _name;
                else paramList = "";

                if (string.IsNullOrEmpty(effectName))
                    return;

                string[] pars = paramList.Split(';');
                float unit = 1.0f; //4 * (60.0f / bpm);

                EffectDef def = null;
                switch (effectName)
                {
                    case "BitCrusher":
                    {
                        float reduction = 4;
                        if (pars.Length > 0) reduction = float.Parse(pars[0]);
                        def = new BitCrusherEffectDef(1.0f, reduction / 44100.0f);
                    } break;

                    case "Retrigger":
                    {
                        int step = 8;
                        if (pars.Length > 0) step = int.Parse(pars[0]);
                        def = new RetriggerEffectDef(1.0f, 0.7f, unit / step);
                    } break;

                    case "Gate":
                    {
                        int step = 8;
                        if (pars.Length > 0) step = int.Parse(pars[0]);
                        def = new GateEffectDef(1.0f, 0.7f, unit / step);
                    } break;
                    
                    case "SideChain":
                    {
                        int step = 4;
                        def = new SideChainEffectDef(1.0f, 1.0f, unit / step);
                    } break;
                    
                    case "Wobble":
                    {
                        int step = 12;
                        if (pars.Length > 0) step = int.Parse(pars[0]);
                        def = new WobbleEffectDef(1.0f, unit / step);
                    } break;

                    case "TapeStop":
                    {
                        int speed = 50;
                        if (pars.Length > 0) speed = int.Parse(pars[0]);
                        def = new TapeStopEffectDef(1.0f, 16.0f / MathL.Max(speed, 1));
                    } break;

                    case "Flanger":
                    {
                        def = new FlangerEffectDef(1.0f);
                    } break;

                    case "Phaser":
                    {
                        def = new PhaserEffectDef(0.5f);
                    } break;
                }

                chart.FxDefines[effect] = def;
                if (def == null)
                {
                    Logger.Log($"KSH2VOLTEX: Failed to create effect info from { effect }");
                }
            }
            
            void TryAddBuiltInFilter(string effectName)
            {
                EffectDef def = null;
                switch (effectName)
                {
                    case "hpf1": def = EffectDef.GetDefault(EffectType.HighPassFilter); break;
                    case "lpf1": def = EffectDef.GetDefault(EffectType.LowPassFilter); break;
                    case "peak": def = EffectDef.GetDefault(EffectType.PeakingFilter); break;
                    case "fx;bitc":
                    case "bitc": def = EffectDef.GetDefault(EffectType.BitCrush); break;
                }

                chart.FilterDefines[effectName] = def;
                if (def == null)
                {
                    Logger.Log($"KSH2VOLTEX: Failed to create effect info from { effectName }");
                }
            }

            TryAddBuiltInFilter(chart.Metadata.FilterType);

            var block = new Block();
            var tick = new Tick();

            float mrBpm = 120.0f;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                line = line.Trim();

                if (line[0] == '#')
                {
                    Dictionary<string, EffectParam> GetParameterList(string args, out string typeName)
                    {
                        var result = new Dictionary<string, EffectParam>();
                        typeName = null;

                        foreach (string a in args.Split(';'))
                        {
                            if (!a.TrySplit('=', out string k, out string v))
                                continue;

                            k = k.Trim();
                            v = v.Trim();

                            if (k == "type")
                            {
                                Logger.Log($"ksh fx type: { v }");
                                typeName = v;
                            }
                            else
                            {
                                // NOTE(local): We aren't worried about on/off state for now, if ever
                                if (v.Contains('>')) v = v.Substring(v.IndexOf('>') + 1).Trim();
                                bool isRange = v.TrySplit('-', out string v0, out string v1);

                                // TODO(local): this will ONLY allow ranges of the same type, so 0.5-1/8 is illegal (but are these really ever used?)
                                // (kinda makes sense for Hz-kHz but uh shh)
                                EffectParam pv;
                                if (v.Contains("on") || v.Contains("off"))
                                {
                                    if (isRange)
                                        pv = new EffectParamF(v0.Contains("on") ? 1 : 0,
                                            v1.Contains("on") ? 1 : 0,
                                            (l, r, t) => MathL.RoundToInt(MathL.Lerp(l, r, t)));
                                    else pv = new EffectParamF(v.Contains("on") ? 1 : 0);
                                }
                                else if (v.Contains('/'))
                                {
                                    if (isRange)
                                    {
                                        pv = new EffectParamX(
                                            int.Parse(v0.Substring(v0.IndexOf('/') + 1)),
                                            int.Parse(v1.Substring(v1.IndexOf('/') + 1)));
                                    }
                                    else pv = new EffectParamX(int.Parse(v.Substring(v.IndexOf('/') + 1)));
                                }
                                else if (v.Contains('%'))
                                {
                                    if (isRange)
                                        pv = new EffectParamF(int.Parse(v0.Substring(0, v0.IndexOf('%'))) / 100.0f,
                                            int.Parse(v1.Substring(0, v1.IndexOf('%'))) / 100.0f);
                                    else pv = new EffectParamF(int.Parse(v.Substring(0, v.IndexOf('%'))) / 100.0f);
                                }
                                else if (v.Contains("samples"))
                                {
                                    if (isRange)
                                        pv = new EffectParamF(int.Parse(v0.Substring(0, v0.IndexOf("samples"))) / 44100.0f,
                                            int.Parse(v1.Substring(0, v1.IndexOf("samples"))) / 44100.0f);
                                    else pv = new EffectParamF(int.Parse(v.Substring(0, v.IndexOf("samples"))) / 44100.0f);
                                }
                                else if (v.Contains("ms"))
                                {
                                    if (isRange)
                                        pv = new EffectParamF(int.Parse(v0.Substring(0, v0.IndexOf("ms"))) / 1000.0f,
                                            int.Parse(v1.Substring(0, v1.IndexOf("ms"))) / 1000.0f);
                                    else pv = new EffectParamF(int.Parse(v.Substring(0, v.IndexOf("ms"))) / 1000.0f);
                                }
                                else if (v.Contains("s"))
                                {
                                    if (isRange)
                                        pv = new EffectParamF(int.Parse(v0.Substring(0, v0.IndexOf("s"))) / 1000.0f,
                                            int.Parse(v1.Substring(0, v1.IndexOf("s"))) / 1000.0f);
                                    else pv = new EffectParamF(int.Parse(v.Substring(0, v.IndexOf("s"))) / 1000.0f);
                                }
                                else if (v.Contains("kHz"))
                                {
                                    if (isRange)
                                        pv = new EffectParamF(float.Parse(v0.Substring(0, v0.IndexOf("kHz"))) * 1000.0f,
                                            float.Parse(v1.Substring(0, v1.IndexOf("kHz"))) * 1000.0f);
                                    else pv = new EffectParamF(float.Parse(v.Substring(0, v.IndexOf("kHz"))) * 1000.0f);
                                }
                                else if (v.Contains("Hz"))
                                {
                                    if (isRange)
                                        pv = new EffectParamF(float.Parse(v0.Substring(0, v0.IndexOf("Hz"))),
                                            float.Parse(v1.Substring(0, v1.IndexOf("Hz"))));
                                    else pv = new EffectParamF(float.Parse(v.Substring(0, v.IndexOf("Hz"))));
                                }
                                else if (v.Contains("dB"))
                                {
                                    if (isRange)
                                        pv = new EffectParamF(float.Parse(v0.Substring(0, v0.IndexOf("dB"))),
                                            float.Parse(v1.Substring(0, v1.IndexOf("dB"))));
                                    else pv = new EffectParamF(float.Parse(v.Substring(0, v.IndexOf("dB"))));
                                }
                                else if (float.TryParse(isRange ? v0 : v, out float floatValue))
                                {
                                    if (isRange)
                                        pv = new EffectParamF(floatValue, float.Parse(v1));
                                    else pv = new EffectParamF(floatValue);
                                }
                                else pv = new EffectParamS(v);

                                Logger.Log($"  ksh fx param: { k } = { pv }");
                                result[k] = pv;
                            }
                        }
                        return result;
                    }

                    if (!line.TrySplit(' ', out string defKind, out string defKey, out string argList))
                        continue;

                    EffectDef def = null;
                    Logger.Log($">> ksh { defKind } \"{ defKey }\"");

                    var pars = GetParameterList(argList, out string effectType);
                    T GetEffectParam<T>(string parsKey, T parsDef) where T : EffectParam
                    {
                        if (pars.TryGetValue(parsKey, out var parsValue) && parsValue is T valueT)
                            return valueT;
                        return parsDef;
                    }
                    switch (effectType)
                    {
                        case "Retrigger":
                        {
                            // TODO(local): updateTrigger, the system doesn't support it yet
                            def = new RetriggerEffectDef(
                                GetEffectParam<EffectParamF>("mix", 1.0f),
                                GetEffectParam<EffectParamF>("rate", 0.7f),
                                GetEffectParam<EffectParamF>("waveLength", 0.25f)
                                );
                        } break;

                        case "Gate":
                        {
                            def = new GateEffectDef(
                                GetEffectParam<EffectParamF>("mix", 1.0f),
                                GetEffectParam<EffectParamF>("rate", 0.7f),
                                GetEffectParam<EffectParamF>("waveLength", 0.25f)
                                );
                        }
                        break;

                        case "Flanger":
                        {
                            def = new FlangerEffectDef(GetEffectParam<EffectParamF>("mix", 1.0f));
                        }
                        break;

                        case "BitCrusher":
                        {
                            def = new BitCrusherEffectDef(
                                GetEffectParam<EffectParamF>("mix", 1.0f),
                                GetEffectParam<EffectParamF>("reduction", 4.0f / 44100.0f)
                                );
                        }
                        break;

                        case "Phaser":
                        {
                            def = new PhaserEffectDef(GetEffectParam<EffectParamF>("mix", 0.5f));
                        }
                        break;

                        case "Wobble":
                        {
                            def = new WobbleEffectDef(
                                GetEffectParam<EffectParamF>("mix", 1.0f),
                                GetEffectParam<EffectParamF>("waveLength", 1.0f / 12)
                                );
                        }
                        break;

                        case "TapeStop":
                        {
                            def = new TapeStopEffectDef(
                                GetEffectParam<EffectParamF>("mix", 1.0f),
                                GetEffectParam<EffectParamF>("speed", 50.0f)
                                );
                        }
                        break;

                        case "SideChain":
                        {
                            def = new SideChainEffectDef(
                                GetEffectParam<EffectParamF>("mix", 1.0f),
                                1.0f,
                                GetEffectParam<EffectParamF>("waveLength", 50.0f)
                                );
                        }
                        break;
                    }
                    
                    if (defKind == "#define_fx")
                        chart.FxDefines[defKey] = def;
                    else if (defKind == "#define_filter")
                        chart.FilterDefines[defKey] = def;
                }
                if (line.TrySplit('=', out string key, out string value))
                {
                    if (key == "t") mrBpm = float.Parse(value);
                    // defined fx should probably be named different than the defaults,
                    //  so it's like slightly safe to assume that failing to create
                    //  a built-in definition from this for either means its a defined effect?
                    if (key == "fx-l" || key == "fx-r")
                        TryAddBuiltInFx(value, mrBpm);
                    else if (key == "filtertype")
                        TryAddBuiltInFilter(value);

                    tick.Settings.Add(new TickSetting(key, value));
                }
                if (line == SEP)
                {
                    chart.m_blocks.Add(block);
                    block = new Block();
                }
                else
                {
                    if (!line.TrySplit('|', out string bt, out string fx, out string vol))
                        continue;

                    if (vol.Length > 2)
                    {
                        string add = vol.Substring(2);
                        vol = vol.Substring(0, 2);

                        if (add.Length >= 2)
                        {
                            string[] args = add.Substring(2).Split(';');

                            char c = add[0];
                            switch (c)
                            {
                                case '@':
                                {
                                    char d = add[1];
                                    switch (d)
                                    {
                                        case '(': case ')': tick.Add.Kind = AddKind.Spin; break;
                                        case '<': case '>': tick.Add.Kind = AddKind.Swing; break;
                                    }
                                    switch (d)
                                    {
                                        case '(': case '<': tick.Add.Direction = -1; break;
                                        case ')': case '>': tick.Add.Direction =  1; break;
                                    }
                                    ParseArg(0, out tick.Add.Duration);
                                    tick.Add.Amplitude = 100;
                                } break;
                                
                                case 'S':
                                {
                                    char d = add[1];
                                    tick.Add.Kind = AddKind.Wobble;
                                    tick.Add.Direction = d == '<' ? -1 : (d == '>' ? 1 : 0);
                                    ParseArg(0, out tick.Add.Duration);
                                    ParseArg(1, out tick.Add.Amplitude);
                                    ParseArg(2, out tick.Add.Frequency);
                                    ParseArg(3, out tick.Add.Decay);
                                } break;
                            }

                            void ParseArg(int i, out int v)
                            {
                                if (args.Length > i) int.TryParse(args[i], out v);
                                else v = 0;
                            }
                        }
                    }

                    for (int i = 0; i < MathL.Min(4, bt.Length); i++)
                    {
                        char c = bt[i];
                        switch (c)
                        {
                            case '0': tick.Bt[i].State = ButtonState.Off; break;
                            case '1': tick.Bt[i].State = ButtonState.Chip; break;
                            case '2': tick.Bt[i].State = ButtonState.Hold; break;
                        }
                    }

                    for (int i = 0; i < MathL.Min(2, fx.Length); i++)
                    {
                        char c = fx[i];
                        switch (c)
                        {
                            case '0': tick.Fx[i].State = ButtonState.Off; break;
                            case '1': tick.Fx[i].State = ButtonState.Hold; break;
                            case '2': tick.Fx[i].State = ButtonState.Chip; break;
                            case '3': tick.Fx[i].State = ButtonState.ChipSample; break;
                                
                            default:
                            {
                                var kind = (FxKind)c;
                                if (Enum.IsDefined(typeof(FxKind), kind) && kind != FxKind.None)
                                {
                                    tick.Fx[i].State = ButtonState.Hold;
                                    tick.Fx[i].FxKind = kind;
                                }
                            } break;
                        }
                    }

                    for (int i = 0; i < MathL.Min(2, vol.Length); i++)
                    {
                        char c = vol[i];
                        switch (c)
                        {
                            case '-': tick.Laser[i].State = LaserState.Inactive; break;
                            case ':': tick.Laser[i].State = LaserState.Lerp; break;
                            default:
                            {
                                tick.Laser[i].State = LaserState.Position;
                                tick.Laser[i].Position.Image = c;
                            } break;
                        }
                    }

                    block.Ticks.Add(tick);
                    tick = new Tick();
                }
            }

            return chart;
        }

        public ChartMetadata Metadata;

        private List<Block> m_blocks = new List<Block>();
        public Tick this[int block, int tick] => m_blocks[block][tick];

        public readonly Dictionary<string, EffectDef> FxDefines = new Dictionary<string, EffectDef>();
        public readonly Dictionary<string, EffectDef> FilterDefines = new Dictionary<string, EffectDef>();
        
        public int BlockCount => m_blocks.Count;

        IEnumerator<TickRef> IEnumerable<TickRef>.GetEnumerator() => new TickEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TickRef>)this).GetEnumerator();

        class TickEnumerator : IEnumerator<TickRef>
        {
            private Chart m_chart;
            private int m_block, m_tick = -1;
            
            object IEnumerator.Current => Current;
            public TickRef Current => new TickRef()
            {
                Block = m_block,
                Index = m_tick,
                MaxIndex = m_chart.m_blocks[m_block].TickCount,
                Tick = m_chart[m_block, m_tick],
            };

            public TickEnumerator(Chart c)
            {
                m_chart = c;
            }

            public void Dispose() => m_chart = null;

            public bool MoveNext()
            {
                if (m_tick == m_chart.m_blocks[m_block].TickCount - 1)
                {
                    m_block++;
                    m_tick = 0;

                    return m_block < m_chart.m_blocks.Count;
                }
                else m_tick++;

                return true;
            }

            public void Reset()
            {
                m_block = 0;
                m_tick = 0;
            }
        }
    }
}
