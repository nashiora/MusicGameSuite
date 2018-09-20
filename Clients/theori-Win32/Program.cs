using System;
using theori.Game.States;

namespace theori.Win32
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Host.Init();
            Host.Start(new VoltexChartSelect_KSH());
        }
    }
}
