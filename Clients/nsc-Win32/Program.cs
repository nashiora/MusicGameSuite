using System;

using theori;

using NeuroSonic.Win32.Platform;
using System.IO;
using System.Globalization;

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

            Logger.AddLogFunction(entry => Console.WriteLine($"{ entry.When.ToString(CultureInfo.InvariantCulture) } [{ entry.Priority }]: { entry.Message }"));

            Host.Init(new PlatformWin32());
            // so we can boot into shared mode with it
            Host.RegisterSharedGameMode(NeuroSonicDescription.Instance);
            // but currently, just launch it in standalone
            Host.StartStandalone(NeuroSonicDescription.Instance, args);
        }
    }
}
