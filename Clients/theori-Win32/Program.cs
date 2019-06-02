using System;

using theori.Win32.Platform;

using NeuroSonic;

namespace theori.Win32
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Host.Init(new PlatformWin32());
            // so we can boot into shared mode with it
            Host.RegisterSharedGameMode(NeuroSonicDescription.Instance);
            // but currently, just launch it in standalone
            Host.StartStandalone(NeuroSonicDescription.Instance, args);
        }
    }
}
