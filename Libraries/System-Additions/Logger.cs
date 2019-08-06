using System.Collections.Generic;
using System.Globalization;

namespace System
{
	public enum LogPriority
	{
		Verbose,
		Debug,
		Info,
		Warn,
		Error,
		Critical,
	}

    public sealed class LogEntry
    {
        public DateTime When;
        public string Message;

        public LogPriority Priority;

        public LogEntry(DateTime when, string message, LogPriority priority)
        {
            When = when;
            Message = message;

            Priority = priority;
        }
    }

    public abstract class Logger
    {
        private static readonly List<Action<LogEntry>> logFunctions = new List<Action<LogEntry>>();

        private static bool block = false;

        public static void AddLogFunction(Action<LogEntry> f)
        {
            if (f == null) return;
            logFunctions.Add(f);
        }

        private static void OnLog(LogEntry entry)
        {
            //Diagnostics.Trace.WriteLine($"{ entry.When.ToString(CultureInfo.InvariantCulture) } [{ entry.Priority }]: { entry.Message }");
            foreach (var f in logFunctions)
                f(entry);
        }

        public static void Log(object obj, LogPriority priority = LogPriority.Verbose)
        {
            Log(obj?.ToString() ?? "null", priority);
        }

        public static void Log(string message, LogPriority priority = LogPriority.Verbose)
        {
            if (block) return;

            var entry = new LogEntry(DateTime.UtcNow, message, priority);
            OnLog(entry);
        }

        public static void Block() => block = true;
        public static void Unblock() => block = false;
    }
}
