using theori;
using theori.GameModes;

using NeuroSonic.GamePlay;
using NeuroSonic.IO;
using System;
using theori.Charting.IO;
using NeuroSonic.Charting;
using theori.Charting;
using NeuroSonic.Charting.IO;

namespace NeuroSonic
{
    public sealed class NeuroSonicGameMode : GameMode
    {
        public static GameMode Instance { get; } = new NeuroSonicGameMode();

        public override bool SupportsStandaloneUsage => true;
        public override bool SupportsSharedUsage => true;

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

                case SlamVolumeEvent _: return new SlamVolumeEventSerializer();

                default: return null;
            }
        }
    }
}
