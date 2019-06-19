using theori;

using NeuroSonic.ChartSelect.Landscape;

namespace NeuroSonic.Startup
{
    public class NeuroSonicStandaloneStartup : BaseMenuLayer
    {
        protected override string Title => "NeuroSonic - Main Menu";

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(NextOffset, "Input Method", EnterInputMethod));
            AddMenuItem(new MenuItem(NextOffset, "Input Binding Configuration", EnterBindingConfig));
            AddSpacing();
            AddMenuItem(new MenuItem(NextOffset, "Free Play", EnterFreePlay));
        }

        public override void Init()
        {
            base.Init();
        }

        private void EnterInputMethod()
        {
            var layer = new InputMethodConfigLayer();
            Host.PushLayer(layer);
        }

        private void EnterBindingConfig()
        {
            Layer layer;
            layer = new ControllerConfigurationLayer();
            Host.PushLayer(layer);
        }

        private void EnterFreePlay()
        {
            Host.PushLayer(new LandscapeChartSelectLayer(Plugin.DefaultSkin));
        }
    }
}
