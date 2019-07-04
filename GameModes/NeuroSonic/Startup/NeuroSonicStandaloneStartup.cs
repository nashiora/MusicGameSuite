using System.Numerics;

using theori;
using theori.Graphics;
using theori.Scripting;

using NeuroSonic.ChartSelect.Landscape;
using NeuroSonic.ChartSelect;
using NeuroSonic.Properties;

namespace NeuroSonic.Startup
{
    public class NeuroSonicStandaloneStartup : BaseMenuLayer
    {
        protected override string Title => Strings.SecretMenu_MainTitle;

        private BasicSpriteRenderer m_renderer;
        private LuaScript m_script;

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(NextOffset, Strings.SecretMenu_InputMethodTitle, EnterInputMethod));
            AddMenuItem(new MenuItem(NextOffset, Strings.SecretMenu_InputBindingConfigTitle, EnterBindingConfig));
            AddMenuItem(new MenuItem(NextOffset, Strings.SecretMenu_ChartManagementTitle, EnterChartManagement));
            //AddSpacing();
            //AddMenuItem(new MenuItem(NextOffset, "Free Play", EnterFreePlay));
        }

        public override void Init()
        {
            base.Init();

            m_script = new LuaScript();

            m_renderer = new BasicSpriteRenderer(Plugin.DefaultResourceLocator, new Vector2(Window.Width, Window.Height));
            m_script["gfx"] = m_renderer;

            //m_script.DoString("function Draw() gfx.SetColor(255, 0, 255); gfx.FillRect(10, 10, 100, 100); end");
            m_script.LoadFile(Plugin.DefaultResourceLocator.OpenFileStream("scripts/chart_select/main.lua"));
        }

        public override void Destroy()
        {
            m_renderer.Dispose();
            base.Destroy();
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

        private void EnterChartManagement()
        {
            Host.PushLayer(new ChartManagerLayer());
        }

        private void EnterFreePlay()
        {
            Host.PushLayer(new LandscapeChartSelectLayer(Plugin.DefaultResourceLocator));
        }

        public override void Update(float delta, float total)
        {
            base.Update(delta, total);
            m_script.Call("Update", delta);
        }

        public override void Render()
        {
            base.Render();
            m_renderer.BeginFrame();
            m_script.Call("Draw");
            m_renderer.Flush();
            m_renderer.EndFrame();
        }
    }
}
