using System;
using theori.Game.States;

namespace theori.Win32
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Init();
            Application.Start(new VoltexChartSelect_KSH());
        }
    }
}
