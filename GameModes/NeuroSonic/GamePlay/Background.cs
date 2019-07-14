using System;
using System.Numerics;

using MoonSharp.Interpreter;

using theori.Graphics;
using theori.Resources;
using theori.Scripting;

namespace NeuroSonic.GamePlay
{
    public class ScriptableBackground : Disposable
    {
        public float HorizonHeight { get; set; }
        public float CombinedTilt { get; set; }
        public float EffectRotation { get; set; }
        public float SpinTimer { get; set; }
        public float SwingTimer { get; set; }

        private LuaScript m_script;

        private ClientResourceManager m_resources;
        private BasicSpriteRenderer m_renderer;

        public ScriptableBackground(ClientResourceLocator locator)
        {
            m_resources = new ClientResourceManager(locator);
        }

        protected override void DisposeManaged()
        {
            m_resources.Dispose();
        }

        public bool AsyncLoad()
        {
            m_script = new LuaScript();

            m_script.LoadFile(Plugin.DefaultResourceLocator.OpenFileStream("scripts/game/main.lua"));
            m_script["window"] = new ScriptWindowInterface();
            m_script["res"] = m_resources;

            m_script.CallIfExists("AsyncLoad");

            if (!m_resources.LoadAll())
                return false;

            return true;
        }

        public bool AsyncFinalize()
        {
            m_script.CallIfExists("AsyncFinalize");

            m_renderer = new BasicSpriteRenderer(Plugin.DefaultResourceLocator, new Vector2(Window.Width, Window.Height));
            m_script["g2d"] = m_renderer;

            if (!m_resources.FinalizeLoad())
                return false;

            return true;
        }

        public void Init()
        {
            m_script.CallIfExists("Init");
        }

        public void Update(float delta, float total)
        {
            m_script["HorizonHeight"] = HorizonHeight;
            m_script["CombinedTilt"] = CombinedTilt;
            m_script["EffectRotation"] = EffectRotation;
            m_script["SpinTimer"] = SpinTimer;
            m_script["SwingTimer"] = SwingTimer;

            m_script.Call("Update", delta, total);
        }

        public void Render()
        {
            m_renderer.BeginFrame();
            m_script.Call("Draw");
            m_renderer.Flush();
            m_renderer.EndFrame();
        }
    }
}
