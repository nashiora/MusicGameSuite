using System;
using theori.Game.Scenes;
using theori.Win32.Platform;

namespace theori.Win32
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string chartToLoad = null;
            if (args.Length > 0)
                chartToLoad = args[0];

            Host.Init(new PlatformWin32());
            Host.Start(new EditorCore(chartToLoad));
        }
    }
}
