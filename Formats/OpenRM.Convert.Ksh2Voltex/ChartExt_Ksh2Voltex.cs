using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRM.Audio.Effects;
using OpenRM.Voltex;

namespace OpenRM.Convert
{
    public static class ChartExt_Ksh2Voltex
    {
        class TempButtonState
        {
            public tick_t StartPosition;

            public EffectDef EffectDef;

            public byte SampleIndex = 0xFF;
            public bool UsingSample = false;

            public TempButtonState(tick_t pos)
            {
                StartPosition = pos;
            }
        }

        class TempLaserState
        {
            public ControlPoint ControlPoint;

            public tick_t StartPosition;
            public float StartAlpha;
            
            public EffectDef EffectDef;

            public TempLaserState(tick_t pos, ControlPoint cp)
            {
                StartPosition = pos;
                ControlPoint = cp;
            }
        }

        public static Chart ToVoltex(this KShootMania.Chart ksh)
        {
            EffectDef ParseFilterType(string str)
            {
                switch (str)
                {
                    case "hpf1": return EffectDef.GetDefault(EffectType.HighPassFilter);
                    case "lpf1": return EffectDef.GetDefault(EffectType.LowPassFilter);
                    case "peak": return EffectDef.GetDefault(EffectType.PeakingFilter);
                    case "fx;bitc":
                    case "bitc": return EffectDef.GetDefault(EffectType.BitCrush);
                        
                    default: return null;
                }
            }

            var voltex = new Chart((int)StreamIndex.COUNT)
            {
                Offset = ksh.Metadata.OffsetMillis / 1_000.0
            };

            {
                if (double.TryParse(ksh.Metadata.BeatsPerMinute, out double bpm))
                    voltex.ControlPoints.Root.BeatsPerMinute = bpm;
                
                var laserParams = voltex[(int)StreamIndex.LaserParams].Add<LaserParamsEvent>(0);
                laserParams.LaserIndex = LaserIndex.Both;
                laserParams.Params.Function = LaserFunction.Source | LaserFunction.Normal;

                var laserGain = voltex[(int)StreamIndex.LaserFilterGain].Add<LaserFilterGainEvent>(0);
                laserGain.LaserIndex = LaserIndex.Both;
                laserGain.Gain = ksh.Metadata.PFilterGain / 100.0f;
                
                var laserFilter = voltex[(int)StreamIndex.LaserFilterKind].Add<LaserFilterKindEvent>(0);
                laserFilter.LaserIndex = LaserIndex.Both;
                laserFilter.FilterEffect = ParseFilterType(ksh.Metadata.FilterType);

                var slamVoume = voltex[(int)StreamIndex.SlamVolume].Add<SlamVolumeEvent>(0);
                slamVoume.Volume = ksh.Metadata.SlamVolume / 100.0f;
            }


            var lastCp = voltex.ControlPoints.Root;
            int lastTsBlock = 0;

            var buttonStates = new TempButtonState[6];
            var laserStates = new TempLaserState[2];

            bool[] laserIsExtended = new bool[2] { false, false };

            foreach (var tickRef in ksh)
            {
                var tick = tickRef.Tick;
                
                int blockOffset = tickRef.Block - lastTsBlock;
                tick_t chartPos = lastCp.Position + blockOffset + (double)tickRef.Index / tickRef.MaxIndex;

                //System.Diagnostics.Trace.WriteLine(chartPos);

                foreach (var setting in tick.Settings)
                {
                    string key = setting.Key;
                    switch (key)
                    {
                        case "pfiltergain":
                        {
                            var laserGain = voltex[(int)StreamIndex.LaserFilterGain].Add<LaserFilterGainEvent>(chartPos);
                            laserGain.LaserIndex = LaserIndex.Both;
                            laserGain.Gain = setting.Value.ToInt() / 100.0f;
                        } break;

                        case "filtertype":
                        {
                            var laserFilter = voltex[(int)StreamIndex.LaserFilterKind].Add<LaserFilterKindEvent>(chartPos);
                            laserFilter.LaserIndex = LaserIndex.Both;
                            laserFilter.FilterEffect = ParseFilterType(setting.Value.ToString());
                        } break;

                        case "chokkakuvol":
                        {
                            var slamVoume = voltex[(int)StreamIndex.SlamVolume].Add<SlamVolumeEvent>(chartPos);
                            slamVoume.Volume = setting.Value.ToInt() / 100.0f;
                        } break;

                        case "laserrange_l": { laserIsExtended[0] = true; } break;
                        case "laserrange_r": { laserIsExtended[1] = true; } break;
                        
                        case "zoom_bottom":
                        {
                            var point = voltex[(int)StreamIndex.Zoom].Add<PathPointEvent>(chartPos);
                            point.Value = setting.Value.ToInt() / 100.0f;
                            //System.Diagnostics.Trace.WriteLine($"ZOOM_BOTTOM @ { chartPos }: { setting.Value } -> { point.Value }");
                        } break;
                        
                        case "zoom_top":
                        {
                            var point = voltex[(int)StreamIndex.Pitch].Add<PathPointEvent>(chartPos);
                            point.Value = setting.Value.ToInt() / 100.0f;
                        } break;
                        
                        case "zoom_side":
                        {
                            var point = voltex[(int)StreamIndex.Offset].Add<PathPointEvent>(chartPos);
                            point.Value = setting.Value.ToInt() / 100.0f;
                        } break;
                        
                        case "roll":
                        {
                            var point = voltex[(int)StreamIndex.Roll].Add<PathPointEvent>(chartPos);
                            point.Value = setting.Value.ToInt() / 360.0f;
                        } break;
                    }
                }

                for (int b = 0; b < 6; b++)
                {
                    bool isFx = b >= 4;
                    
                    var data = isFx ? tick.Fx[b - 4] : tick.Bt[b];
                    var fxKind = data.FxKind;

                    void CreateHold(tick_t endPos)
                    {
                        var state = buttonStates[b];

                        var startPos = state.StartPosition;
                        var button = voltex[b].Add<ButtonObject>(startPos, endPos - startPos);
                        //System.Diagnostics.Trace.WriteLine($"{ endPos } - { startPos } = { endPos - startPos }");
                    }

                    switch (data.State)
                    {
                        case KShootMania.ButtonState.Off:
                        {
                            if (buttonStates[b] != null)
                                CreateHold(chartPos);
                            buttonStates[b] = null;
                        } break;

                        case KShootMania.ButtonState.Chip:
                        case KShootMania.ButtonState.ChipSample:
                        {
                            //System.Diagnostics.Trace.WriteLine(b);
                            voltex[b].Add<ButtonObject>(chartPos);
                        } break;
                        
                        case KShootMania.ButtonState.Hold:
                        {
                            if (buttonStates[b] == null)
                            {
                                buttonStates[b] = new TempButtonState(chartPos);
                            }
                        } break;
                    }
                }

                for (int l = 0; l < 2; l++)
                {
                    var data = tick.Laser[l];
                    var state = data.State;

                    tick_t CreateSegment(tick_t endPos, float endAlpha)
                    {
                        var startPos = laserStates[l].StartPosition;
                        float startAlpha = laserStates[l].StartAlpha;

                        var duration = endPos - startPos;
                        if (duration <= tick_t.FromFraction(1, 32))
                            duration = 0;

                        var analog = voltex[l + 6].Add<AnalogObject>(startPos, duration);
                        //System.Diagnostics.Trace.WriteLine($"{ startPos } -> { endPos } ({ duration }) :: { startAlpha }, { endAlpha }");
                        analog.InitialValue = startAlpha;
                        analog.FinalValue = endAlpha;
                        analog.RangeExtended = laserIsExtended[l];

                        return startPos + duration;
                    }

                    switch (state)
                    {
                        case KShootMania.LaserState.Inactive:
                        {
                            if (laserStates[l] != null)
                            {
                                laserStates[l] = null;
                                laserIsExtended[l] = false;
                            }
                        } break;

                        case KShootMania.LaserState.Lerp:
                        {
                        } break;
                        
                        case KShootMania.LaserState.Position:
                        {
                            var alpha = data.Position;
                            var startPos = chartPos;

                            if (laserStates[l] != null)
                                startPos = CreateSegment(chartPos, alpha.Alpha);

                            laserStates[l] = new TempLaserState(startPos, lastCp)
                            {
                                StartAlpha = alpha.Alpha,
                            };
                        } break;
                    }
                }

                switch (tick.Add.Kind)
                {
                    case KShootMania.AddKind.None: break;

                    case KShootMania.AddKind.Spin:
                    {
                        tick_t duration = tick_t.FromFraction(tick.Add.Duration * 2, 192);
                        var spin = voltex[(int)StreamIndex.HighwayEffect].Add<SpinImpulseEvent>(chartPos, duration);
                        spin.Direction = (AngularDirection)tick.Add.Direction;
                    } break;

                    case KShootMania.AddKind.Swing:
                    {
                        tick_t duration = tick_t.FromFraction(tick.Add.Duration * 2, 192);
                        var swing = voltex[(int)StreamIndex.HighwayEffect].Add<SwingImpulseEvent>(chartPos, duration);
                        swing.Direction = (AngularDirection)tick.Add.Direction;
                        swing.Amplitude = tick.Add.Amplitude * 70 / 100.0f;
                    } break;

                    case KShootMania.AddKind.Wobble:
                    {
                        tick_t duration = tick_t.FromFraction(tick.Add.Duration, 192);
                        var wobble = voltex[(int)StreamIndex.HighwayEffect].Add<WobbleImpulseEvent>(chartPos, duration);
                        wobble.Direction = (LinearDirection)tick.Add.Direction;
                        wobble.Amplitude = tick.Add.Amplitude / 250.0f;
                        wobble.Decay = (Decay)tick.Add.Decay;
                        wobble.Frequency = tick.Add.Frequency;
                    } break;
                }
            }

            return voltex;
        }
    }
}
