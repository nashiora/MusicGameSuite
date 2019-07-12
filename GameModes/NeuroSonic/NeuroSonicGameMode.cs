using theori;
using theori.GameModes;

using NeuroSonic.GamePlay;
using NeuroSonic.IO;
using System;
using theori.Charting.IO;
using NeuroSonic.Charting;
using theori.Charting;
using NeuroSonic.Charting.IO;
using System.Collections.Generic;

namespace NeuroSonic
{
    public sealed class NeuroSonicGameMode : GameMode
    {
        public static GameMode Instance { get; } = new NeuroSonicGameMode();

        public override bool SupportsStandaloneUsage => true;
        public override bool SupportsSharedUsage => true;

        private readonly Dictionary<(int, Type), ChartObjectSerializer> m_objectSerializer =
            new Dictionary<(int, Type), ChartObjectSerializer>();

        public NeuroSonicGameMode()
            : base("NeuroSonic")
        {
        }

        public override void InvokeStandalone(string[] args) => Plugin.NSC_Main(args);
        public override Layer CreateSharedGameLayer() => new GameLayer(null, null, null, AutoPlay.None);

        public override ChartObjectSerializer GetSerializerByID(int id)
        {
            switch (id)
            {
                case 1: return new ButtonObjectSerializer();
                case 2: return new AnalogObjectSerializer();

                case StreamIndex.LaserApplication: return new LaserApplicationEventSerializer();
                case StreamIndex.LaserParams: return new LaserParamsEventSerializer();
                // NOTE: this case should be identical to PathPointEventSerializer::ID
                case StreamIndex.Zoom: return new PathPointEventSerializer();
                case StreamIndex.EffectKind: return new EffectKindEventSerializer();
                case StreamIndex.LaserFilterKind: return new LaserFilterKindEventSerializer();
                case StreamIndex.LaserFilterGain: return new LaserFilterGainEventSerializer();
                case StreamIndex.SlamVolume: return new SlamVolumeEventSerializer();

                default: return null;
            }
        }

        public override ChartObjectSerializer GetSerializerFor(ChartObject obj)
        {
            switch (obj)
            {
                case ButtonObject _: return new ButtonObjectSerializer();
                case AnalogObject _: return new AnalogObjectSerializer();

                case LaserApplicationEvent _: return new LaserApplicationEventSerializer();
                case LaserParamsEvent _: return new LaserParamsEventSerializer();
                case PathPointEvent _: return new PathPointEventSerializer();
                case EffectKindEvent _: return new EffectKindEventSerializer();
                case LaserFilterKindEvent _: return new LaserFilterKindEventSerializer();
                case LaserFilterGainEvent _: return new LaserFilterGainEventSerializer();
                case SlamVolumeEvent _: return new SlamVolumeEventSerializer();

                default: return null;
            }
        }
    }
}
