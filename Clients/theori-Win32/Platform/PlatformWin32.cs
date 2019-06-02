using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using theori.IO;
using theori.Platform;

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace theori.Win32.Platform
{
    public sealed class PlatformWin32 : IPlatform
    {
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
