using System;
using theori.IO;

namespace theori.Platform
{
    public interface IPlatform
    {
        IntPtr LoadLibrary(string libraryName);
        void FreeLibrary(IntPtr library);
        IntPtr GetProcAddress(IntPtr library, string procName);

        OpenFileResult ShowOpenFileDialog(OpenFileDialogDesc desc);
        FolderBrowserResult ShowFolderBrowserDialog(FolderBrowserDialogDesc desc);
    }
}
