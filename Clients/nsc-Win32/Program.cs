using System;
using System.IO;
using System.Globalization;

using theori;

using NeuroSonic.Win32.Platform;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NeuroSonic.Win32
{
    static class TempFileWriter
    {
        const string FILE_NAME = "nsc-log.txt";

        private static readonly List<string> lines = new List<string>();

        private static readonly object flushLock = new object();

        public static void EmptyFile()
        {
            lock (flushLock)
            {
                File.Delete(FILE_NAME);
                File.WriteAllText(FILE_NAME, "");
            }
        }

        public static void WriteLine(string line)
        {
            lines.Add(line);
        }

        public static void Flush()
        {
            if (lines.Count == 0) return;

            lock (flushLock)
            {
                int count = lines.Count;
                using (var writer = new StreamWriter(File.Open(FILE_NAME, FileMode.Append)))
                {
                    for (int i = 0; i < count; i++)
                        writer.WriteLine(lines[i]);
                }
                lines.RemoveRange(0, count);
            }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            string cd = System.Reflection.Assembly.GetEntryAssembly().Location;
            while (cd != null && !Directory.Exists(Path.Combine(cd, "InstallDir")))
                cd = Directory.GetParent(cd)?.FullName;

            if (cd != null && Directory.Exists(Path.Combine(cd, "InstallDir")))
                Environment.CurrentDirectory = Path.Combine(cd, "InstallDir");
#endif

#if !DEBUG
            try
            {
#endif
            Host.Platform = new PlatformWin32();

            TempFileWriter.EmptyFile();

            Logger.AddLogFunction(entry => Console.WriteLine($"{ entry.When.ToString(CultureInfo.InvariantCulture) } [{ entry.Priority }]: { entry.Message }"));
            Logger.AddLogFunction(entry => TempFileWriter.WriteLine($"{ entry.When.ToString(CultureInfo.InvariantCulture) } [{ entry.Priority }]: { entry.Message }"));

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(100);
                    TempFileWriter.Flush();
                }
            });

            if (Environment.Is64BitProcess)
            {
                Host.Platform.LoadLibrary("x64/SDL2.dll");
                Host.Platform.LoadLibrary("x64/freetype6.dll");
            }
            else
            {
                Host.Platform.LoadLibrary("x86/SDL2.dll");
                Host.Platform.LoadLibrary("x86/freetype6.dll");
            }

            Host.DefaultInitialize();

            Host.OnUserQuit += TempFileWriter.Flush;
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
