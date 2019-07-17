using theori;
using theori.IO;
using theori.Resources;
using theori.Scripting;

namespace NeuroSonic.ChartSelect
{
    public class ChartSelectLayer : NscLayer
    {
        private readonly ClientResourceLocator m_locator;

        private ClientResourceManager m_resources;

        private AsyncLoader m_loader;
        private LuaScript m_script;

        public ChartSelectLayer(ClientResourceLocator locator)
        {
            m_locator = locator;
            m_resources = new ClientResourceManager(locator);
        }

        public override void Init()
        {
            base.Init();

            m_script = new LuaScript();
            m_script.LoadFile(m_locator.OpenFileStream("scripts/chart_select/main.lua"));
            m_script.InitResourceLoading(m_locator);
            m_script.InitSpriteRenderer(m_locator);

            m_loader = new AsyncLoader();

            m_script.CallIfExists("Init");
        }

        public override void Destroy()
        {
            base.Destroy();

            m_script.Dispose();
            m_script = null;

            m_resources.Dispose();
            m_resources = null;
        }

        public override void Update(float delta, float total)
        {
            base.Update(delta, total);

            m_loader.LoadAll();
            m_script.Update(delta, total);
        }

        public override void Render()
        {
            m_script.Draw();
        }

        public override bool KeyPressed(KeyInfo info)
        {
            switch (info.KeyCode)
            {
                case KeyCode.ESCAPE:
                {
                    Host.PopToParent(this);
                } break;

                default: return false;
            }

            return true;
        }

        protected internal override bool ControllerButtonPressed(ControllerInput input)
        {
            switch (input)
            {
                default: return false;
            }

            return true;
        }

    }
}
