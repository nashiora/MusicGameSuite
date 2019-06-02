using theori.IO;

namespace theori.Platform
{
    public interface IPlatform
    {
        OpenFileResult ShowOpenFileDialog(OpenFileDialogDesc desc);
    }
}
