using theori;

using NeuroSonic.GamePlay;

namespace NeuroSonic.Startup
{
    public class NeuroSonicStandaloneStartup : BaseMenuLayer
    {
        protected override string Title => "NeuroSonic - Main Menu";

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(ItemIndex, "Input Method", EnterInputMethod));
            AddSpacing();
            AddMenuItem(new MenuItem(ItemIndex, "Free Play", EnterFreePlay));
            AddMenuItem(new MenuItem(ItemIndex, "Demo Mode", EnterDemoMode));
        }

        private void EnterInputMethod()
        {
            var layer = new InputMethodConfigLayer();
            Host.PushLayer(layer);
        }

        private void EnterFreePlay()
        {
            var gameLayer = new GameLayer(AutoPlay.None);
            Host.PushLayer(gameLayer, _ => gameLayer.OpenChart());
        }

        private void EnterDemoMode()
        {
            var gameLayer = new GameLayer(AutoPlay.ButtonsAndLasers);
            // TODO(local): add demo controller as new layer on top!
            // Demo controller could also subclas or something, but I want to make sure
            //  this kind of use-case works. An otherwise invisible layer controls the parent
            //  layer silently, and is still handled properly as if it wasn't there c:
            // NOT an overlay, though, overlays are special and will ofc work if considered.
            Host.PushLayer(gameLayer);
        }
    }
}
