using theori;

using NeuroSonic.GamePlay;
using NeuroSonic.Startup;

namespace NeuroSonic
{
    internal static class Plugin
    {
        public static string[] ProgramArgs { get; private set; }

        /// <summary>
        /// Invoked when the plugin starts in Standalone.
        /// </summary>
        public static void NSC_Main(string[] args)
        {
            ProgramArgs = args;

            // TODO(local): push the game loading layer, which creates the game layer
            //Host.PushLayer(new GameLayer(true));
            Host.PushLayer(new NeuroSonicStandaloneStartup());
        }
    }
}
