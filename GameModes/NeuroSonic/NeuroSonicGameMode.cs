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

        private readonly Dictionary<(int, Type), ChartObjectSerializer> m_objectSerializers =
            new Dictionary<(int, Type), ChartObjectSerializer>();

        public NeuroSonicGameMode()
            : base("NeuroSonic")
        {
            void AddSerializer<TObj>(ChartObjectSerializer<TObj> ser)
                where TObj : Entity
            {
                int actId = ser.ID;
                var actType = typeof(TObj);

#if DEBUG
                foreach (var (oid, otype) in m_objectSerializers.Keys)
                {
                    System.Diagnostics.Debug.Assert(oid != actId, "Cannot have two duplicate IDs boi");
                    System.Diagnostics.Debug.Assert(otype != actType, "Cannot have two duplicate types boi");
                }
#endif

                m_objectSerializers[(actId, actType)] = ser;
            }

            AddSerializer(new ButtonObjectSerializer(1));
            AddSerializer(new AnalogObjectSerializer(2));
            AddSerializer(new LaserApplicationEventSerializer(3));
            AddSerializer(new LaserParamsEventSerializer(4));
            AddSerializer(new PathPointEventSerializer(5));
            AddSerializer(new EffectKindEventSerializer(6));
            AddSerializer(new LaserFilterKindEventSerializer(7));
            AddSerializer(new LaserFilterGainEventSerializer(8));
            AddSerializer(new SlamVolumeEventSerializer(9));
            AddSerializer(new SpinImpulseEventSerializer(10));
            AddSerializer(new SwingImpulseEventSerializer(11));
            AddSerializer(new WobbleImpulseEventSerializer(12));
        }

        public override void InvokeStandalone(string[] args) => Plugin.NSC_Main(args);
        public override Layer CreateSharedGameLayer() => new GameLayer(null, null, null, AutoPlay.None);

        public override ChartFactory CreateChartFactory() => new NeuroSonicChartFactory();

        public override ChartObjectSerializer GetSerializerByID(int id)
        {
            foreach (var (oid, otype) in m_objectSerializers.Keys)
            {
                if (oid == id) return m_objectSerializers[(oid, otype)];
            }
            return null;
        }

        public override ChartObjectSerializer GetSerializerFor(Entity obj)
        {
            foreach (var (oid, otype) in m_objectSerializers.Keys)
            {
                if (otype == obj.GetType()) return m_objectSerializers[(oid, otype)];
            }
            return null;
        }
    }
}
