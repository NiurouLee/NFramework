namespace Main
{
    public static class LogSetting
    {
        public static LogLevel LogLevel = LogLevel.Debug;
        public static bool IsOpenReporter;

    }
    
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
        NoLog = 4,
    }
}