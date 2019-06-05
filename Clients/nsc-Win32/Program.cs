using System;
using System.IO;
using System.Globalization;

using theori;
using theori.Configuration;

using NeuroSonic.Win32.Platform;

namespace NeuroSonic.Win32
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            string cd = System.Reflection.Assembly.GetEntryAssembly().Location;
            while (!Directory.Exists(Path.Combine(cd, "InstallDir")))
                cd = Directory.GetParent(cd).FullName;
            Environment.CurrentDirectory = Path.Combine(cd, "InstallDir");
#endif

            Host.Platform = new PlatformWin32();

            Logger.AddLogFunction(entry => Console.WriteLine($"{ entry.When.ToString(CultureInfo.InvariantCulture) } [{ entry.Priority }]: { entry.Message }"));

            if (!Host.InitGameConfig())
                ;

            if (!Host.InitWindowSystem())
                ;

            if (!Host.InitInputSystem())
                ;

            if (!Host.InitGraphicsPipeline())
                ;

            if (!Host.InitAudioSystem())
                ;

            Host.StartStandalone(NeuroSonicDescription.Instance, args);
        }
    }
}
