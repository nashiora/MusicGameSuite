/*

The MIT License

Copyright (c) 2007-2016 ppy Pty Ltd <contact@ppy.sh>.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

 */

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace System
{
    public static class RuntimeInfo
    {
        [DllImport(@"kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport(@"kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        public static bool Is32Bit { get; }
        public static bool Is64Bit { get; }
        public static bool IsMono { get; }
        public static bool IsWindows { get; }
        public static bool IsUnix { get; }
        public static bool IsLinux { get; }
        public static bool IsMacOsx { get; }
        public static bool IsWine { get; }

        static RuntimeInfo()
        {
            IsMono = Type.GetType("Mono.Runtime") != null;
            int p = (int)Environment.OSVersion.Platform;
            IsUnix = (p == 4) || (p == 6) || (p == 128);
            IsWindows = Path.DirectorySeparatorChar == '\\';

            Is32Bit = IntPtr.Size == 4;
            Is64Bit = IntPtr.Size == 8;

            if (IsUnix)
            {
                Process uname = new Process();
                uname.StartInfo.FileName = "uname";
                uname.StartInfo.UseShellExecute = false;
                uname.StartInfo.RedirectStandardOutput = true;
                uname.Start();
                string output = uname.StandardOutput.ReadToEnd();
                uname.WaitForExit();

                output = output.ToUpper().Replace("\n", "").Trim();

                IsMacOsx = output == "DARWIN";
                IsLinux = output == "LINUX";
            }
            else
            {
                IsMacOsx = false;
                IsLinux = false;
            }

            if (IsWindows)
            {
                IntPtr hModule = GetModuleHandle(@"ntdll.dll");
                if (hModule == IntPtr.Zero)
                    IsWine = false;
                else
                {
                    IntPtr fptr = GetProcAddress(hModule, @"wine_get_version");
                    IsWine = fptr != IntPtr.Zero;
                }
            }
        }
    }
}
