using System;

using theori.Win32.Platform;

using NeuroSonic;
using System.IO;
using System.Globalization;

namespace theori.Win32
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Host.Platform = new PlatformWin32();

            Logger.AddLogFunction(entry => System.Diagnostics.Trace.WriteLine($"{ entry.When.ToString(CultureInfo.InvariantCulture) } [{ entry.Priority }]: { entry.Message }"));

            Host.DefaultInitialize();
            Host.RegisterSharedGameMode(NeuroSonicDescription.Instance);
            Host.StartShared(args);
        }
    }
}
