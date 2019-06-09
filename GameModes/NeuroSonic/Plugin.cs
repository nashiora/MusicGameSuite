using theori;

using NeuroSonic.GamePlay;
using NeuroSonic.Startup;
using System.IO;

namespace NeuroSonic
{
    internal static class Plugin
    {
        public const string NSC_CONFIG_FILE = "nsc-config.ini";

        public static string[] ProgramArgs { get; private set; }

        public static NscConfig Config { get; private set; }

        /// <summary>
        /// Invoked when the plugin starts in Standalone.
        /// </summary>
        public static void NSC_Main(string[] args)
        {
            ProgramArgs = args;

            Host.OnUserQuit += NSC_Quit;

            Config = new NscConfig();
            if (File.Exists(NSC_CONFIG_FILE))
                LoadNscConfig();
            // save the defaults on init
            else SaveNscConfig();

            // TODO(local): push the game loading layer, which creates the game layer
            //Host.PushLayer(new GameLayer(true));
            Host.PushLayer(new NeuroSonicStandaloneStartup());
        }

        private static void NSC_Quit()
        {
            SaveNscConfig();
        }

        private static void LoadNscConfig()
        {
            using (var reader = new StreamReader(File.OpenRead(NSC_CONFIG_FILE)))
                Config.Load(reader);
        }

        public static void LoadConfig()
        {
            LoadNscConfig();
            Host.LoadConfig();
        }

        private static void SaveNscConfig()
        {
            using (var writer = new StreamWriter(File.OpenWrite(NSC_CONFIG_FILE)))
                Config.Save(writer);
        }

        public static void SaveConfig()
        {
            SaveNscConfig();
            Host.SaveConfig();
        }
    }
}
