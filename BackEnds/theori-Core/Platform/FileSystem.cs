namespace theori.Platform
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

    public struct OpenFileResult
    {
        public DialogResult DialogResult;
        public string FilePath;

        public string[] AllResults => FilePath?.Split(';') ?? new string[0];
    }

    public static class FileSystem
    {
        public static OpenFileResult ShowOpenFileDialog(OpenFileDialogDesc desc)
        {
            return Host.Platform.ShowOpenFileDialog(desc);
        }
    }
}
