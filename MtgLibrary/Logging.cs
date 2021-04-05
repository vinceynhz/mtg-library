using System;

using MtgLibrary;

namespace MtgData
{
    public enum LogLevel
    {
        TRACE,
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL
    }
    public delegate void LogDelegate(string message);

    //
    // Summary: This class is basically to a wrapper service that all places can use to direct logging
    //
    public class LoggingService
    {
        public static readonly LoggingService Instance = new LoggingService();

        public LogDelegate OnLog { get; set; }

        private LogLevel level;

        // To prevent instances
        private LoggingService()
        {
            var settingsLevel = Settings.Get(Settings.LOGGING_LEVEL);
            if (settingsLevel != null)
            {
                this.level = Enum.Parse<LogLevel>(settingsLevel);
            }
            this.OnLog = Console.WriteLine;
            this.Log(LogLevel.DEBUG, $"Logging level set to: {this.level.ToString()}");
        }

        public void Log(LogLevel level, string msg)
        {
            if (level >= this.level)
            {
                var toLog = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {level.ToString()} - {msg}";
                this.OnLog(toLog + "\n");
            }
        }

        public void Trace(string msg)
        {
            this.Log(LogLevel.TRACE, msg);
        }

        public void Debug(string msg)
        {
            this.Log(LogLevel.DEBUG, msg);
        }

        public void Info(string msg)
        {
            this.Log(LogLevel.INFO, msg);
        }

        public void Warn(string msg)
        {
            this.Log(LogLevel.WARN, msg);
        }

        public void Error(string msg)
        {
            this.Log(LogLevel.ERROR, msg);
        }

        public void Fatal(string msg)
        {
            this.Log(LogLevel.FATAL, msg);
        }

        public void UpdateLoggingLevel(LogLevel level)
        {
            this.level = level;
            this.Log(LogLevel.DEBUG, $"Logging level set to: {this.level.ToString()}");
        }
    }
}