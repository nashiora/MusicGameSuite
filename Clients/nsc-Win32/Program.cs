using System;
using System.IO;
using System.Globalization;

using theori;

using NeuroSonic.Win32.Platform;

namespace NeuroSonic.Win32
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG || true
            string cd = System.Reflection.Assembly.GetEntryAssembly().Location;
            while (cd != null && !Directory.Exists(Path.Combine(cd, "InstallDir")))
                cd = Directory.GetParent(cd).FullName;

            if (cd != null && Directory.Exists(Path.Combine(cd, "InstallDir")))
                Environment.CurrentDirectory = Path.Combine(cd, "InstallDir");
#endif

#if !DEBUG
            try
            {
#endif
                Host.Platform = new PlatformWin32();

                Logger.AddLogFunction(entry => Console.WriteLine($"{ entry.When.ToString(CultureInfo.InvariantCulture) } [{ entry.Priority }]: { entry.Message }"));

                if (!Host.InitGameConfig())
                    ;

                if (!Host.InitWindowSystem())
                    ;

                if (!Host.InitGraphicsPipeline())
                    ;

                if (!Host.InitAudioSystem())
                    ;

                Host.StartStandalone(NeuroSonicDescription.Instance, args);
#if !DEBUG
            }
            catch (Exception e)
            {
                try
                {
                    Host.Quit();
                }
                catch (Exception e2)
                {
                }

                Console.WriteLine(e);

                Console.WriteLine();
                Console.Write("Press any key to exit.");
                Console.ReadKey();
            }
#endif
        }
    }
}
