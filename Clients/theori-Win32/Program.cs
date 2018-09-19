using theori.Game.States;

namespace theori.Win32
{
    static class Program
    {
        static void Main(string[] args)
        {
            Application.Init();
            Application.Start(new VoltexGameplay());
        }
    }
}
