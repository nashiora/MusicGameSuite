using theori;

using NeuroSonic.GamePlay;

namespace NeuroSonic
{
    internal static class Plugin
    {
        /// <summary>
        /// Invoked when the plugin starts in Standalone.
        /// </summary>
        public static void NSC_Main(string[] args)
        {
            // TODO(local): push the game loading layer, which creates the game layer
            Host.PushLayer(new GameLayer(true));
        }
    }
}
