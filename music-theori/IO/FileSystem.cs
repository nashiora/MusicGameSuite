namespace theori.IO
{
    public enum DialogResult
    {
        None,
        OK, Cancel,
        Abort, Retry,
        Ignore,
        Yes, No,
    }

    public struct FileFilter
    {
        public string Description;
        public string[] Extensions;

        public FileFilter(string desc, params string[] exts)
        {
            Description = desc;
            Extensions = exts;
        }
    }

    public struct OpenFileDialogDesc
    {
        public string Name;
        public FileFilter[] Filters;
        public bool AllowMultiple;

        public OpenFileDialogDesc(string name, FileFilter[] filter, bool allowMulti = false)
        {
            Name = name;
            Filters = filter;
            AllowMultiple = allowMulti;
        }
    }

    public struct FolderBrowserDialogDesc
    {
        public string Name;

        public FolderBrowserDialogDesc(string name)
        {
            Name = name;
        }
    }

    public struct OpenFileResult
    {
        public DialogResult DialogResult;
        public string FilePath;

        public string[] AllResults => FilePath?.Split(';') ?? new string[0];
    }

    public struct FolderBrowserResult
    {
        public DialogResult DialogResult;
        public string FolderPath;
    }

    public static class FileSystem
    {
        public static OpenFileResult ShowOpenFileDialog(OpenFileDialogDesc desc)
        {
            return Host.Platform.ShowOpenFileDialog(desc);
        }
        public static FolderBrowserResult ShowFolderBrowserDialog(FolderBrowserDialogDesc desc)
        {
            return Host.Platform.ShowFolderBrowserDialog(desc);
        }
    }
}
