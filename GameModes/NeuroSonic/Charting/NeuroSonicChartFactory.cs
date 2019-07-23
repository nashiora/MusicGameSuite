using System.Diagnostics;
using theori.Charting;

namespace NeuroSonic.Charting
{
    public sealed class NeuroSonicChartFactory : ChartFactory
    {
        public static readonly NeuroSonicChartFactory Instance = new NeuroSonicChartFactory();

        public override Chart CreateNew()
        {
            var chart = new Chart(NeuroSonicGameMode.Instance);
            for (int i = 0; i < 6; i++)
                chart.CreateTypedLane<ButtonObject>(i, EntityRelation.Equal);
            for (int i = 0; i < 2; i++)
                chart.CreateTypedLane<AnalogObject>(i + 6, EntityRelation.Equal);

            chart.CreateTypedLane<HighwayTypedEvent>(NscLane.HighwayEvent, EntityRelation.Subclass);
            chart.CreateTypedLane<ButtonTypedEvent>(NscLane.ButtonEvent, EntityRelation.Subclass);
            chart.CreateTypedLane<LaserTypedEvent>(NscLane.LaserEvent, EntityRelation.Subclass);

            chart.CreateTypedLane<PathPointEvent>(NscLane.CameraZoom);
            chart.CreateTypedLane<PathPointEvent>(NscLane.CameraPitch);
            chart.CreateTypedLane<PathPointEvent>(NscLane.CameraOffset);
            chart.CreateTypedLane<PathPointEvent>(NscLane.CameraTilt);

            return chart;
        }
    }
}
