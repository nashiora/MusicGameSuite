using System;
using System.Globalization;

using static SDL2.SDL;

namespace theori
{
	public enum LogPriority
	{
		Verbose = 1,
		Debug,
		Info,
		Warn,
		Error,
		Critical,
	}

    public enum LogCategory
    {
        Application = 0,
        Error,
        Assert,
        System,
        Audio,
        Video,
        Render,
        Input,
        Test,

        Plugin = SDL_LOG_CATEGORY_CUSTOM,
    }

    public static class Logger
    {
        static Logger()
        {
            SDL_LogSetOutputFunction(SDL_LogCallback, IntPtr.Zero);
        }

        public static void Log(object obj, LogCategory category = LogCategory.Application, LogPriority priority = LogPriority.Verbose)
        {
            Log(obj?.ToString() ?? "null", category, priority);
        }

        public static void Log(string message, LogCategory category = LogCategory.Application, LogPriority priority = LogPriority.Verbose)
        {
            // TODO(local): actually logging backend here.
            System.Diagnostics.Trace.WriteLine($"{ DateTime.UtcNow.ToString(NumberFormatInfo.InvariantInfo) } [{ category }][{ priority }]: { message }");
        }

        private static unsafe void SDL_LogCallback(IntPtr userdata, int category, SDL_LogPriority priority, IntPtr msgPtr)
        {
            string message = new string((char *)msgPtr.ToPointer());
        }
    }
}
