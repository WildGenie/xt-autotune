using System;

namespace AutoTune.Shared {

    public static class Logger {

        public static event EventHandler<LogEventArgs> Trace;

        public static void Info(string text, params object[] args) {
            Message(LogLevel.Info, text, args);
        }

        public static void Debug(string text, params object[] args) {
            Message(LogLevel.Debug, text, args);
        }

        public static void Error(Exception e, string text, params object[] args) {
            Message(LogLevel.Error, text, args);
            Message(LogLevel.Error, e.Message);
            Message(LogLevel.Debug, e.GetType().Name);
            Message(LogLevel.Debug, e.StackTrace);
        }

        public static void Message(LogLevel level, string text, params object[] args) {
            string message = args.Length > 0 ? string.Format(text, args) : text;
            Trace?.Invoke(typeof(Logger), new LogEventArgs(level, message));
        }
    }
}
