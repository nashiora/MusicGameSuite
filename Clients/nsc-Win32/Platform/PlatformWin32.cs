using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using theori.IO;
using theori.Platform;

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace NeuroSonic.Win32.Platform
{
    internal static class Win32
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    public sealed class PlatformWin32 : IPlatform
    {
        public IntPtr LoadLibrary(string libraryName) => Win32.LoadLibrary(libraryName);
        public void FreeLibrary(IntPtr library) => Win32.FreeLibrary(library);
        public IntPtr GetProcAddress(IntPtr library, string procName) => Win32.GetProcAddress(library, procName);

        public OpenFileResult ShowOpenFileDialog(OpenFileDialogDesc desc)
        {
            Debug.Assert(RuntimeInfo.IsWindows);

            string GetFilterFor(FileFilter filter)
            {
                var exts = filter.Extensions.Select(e => $"*.{ e }");
                return $"{ filter.Description } ({ string.Join(", ", exts) })|{ string.Join(";", exts) }";
            }

            var dialog = new OpenFileDialog()
            {
                Filter = string.Join("|", desc.Filters.Select(GetFilterFor)),
            };

            var result = new OpenFileResult()
            {
                DialogResult = (DialogResult)dialog.ShowDialog(),
            };
            if (result.DialogResult == DialogResult.OK)
            {
                result.FilePath = dialog.FileName;
            }

            return result;
        }
    }
}
