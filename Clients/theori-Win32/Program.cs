using System;
using theori.Game.States;
using theori.Win32.Platform;

namespace theori.Win32
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Host.Init(new PlatformWin32());
            Host.Start(new VoltexChartSelect_KSH());
        }
    }
}
