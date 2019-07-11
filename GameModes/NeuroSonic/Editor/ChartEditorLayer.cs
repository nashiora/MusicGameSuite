using System;

using theori.Charting;
using theori.Charting.IO;

using NeuroSonic.Charting;
using NeuroSonic.Charting.IO;

namespace NeuroSonic.Editor
{
    public class ChartEditorLayer : NscLayer
    {
        private ChartInfo m_chartInfo;

        private Chart m_chart;

        public ChartEditorLayer(ChartInfo chartInfo)
        {
            m_chartInfo = chartInfo;
        }

        public override bool AsyncLoad()
        {
            var chartSerializer = BinaryTheoriChartSerializer.GetSerializerFor(NeuroSonicGameMode.Instance);
            //chartSerializer.DeserializeChart(m_chartInfo);

            return true;
        }

        public override bool AsyncFinalize()
        {
            return true;
        }

        public override void Init()
        {
        }
    }
}
