using Main;
using UnityEngine;
using Object = UnityEngine.Object;

public static class DebugCustom
{
    public static void Log(object message, Object context = null)
    {
        DebugLog(LogLevel.Debug, context, message.ToString());
    }

    public static void LogInfo(object message, Object context = null)
    {
        DebugLog(LogLevel.Info, context, message.ToString());
    }

    public static void LogWarning(object message, Object context = null)
    {
        DebugLog(LogLevel.Warn, context, message.ToString());
    }

    public static void LogError(object message, Object context = null)
    {
        DebugLog(LogLevel.Error, context, message.ToString());
    }

    private static void DebugLog(LogLevel logLevel, Object context, string message)
    {
        if (logLevel < LogSetting.LogLevel)
        {
            return;
        }

        message = $"{Application.productName} {Time.unscaledTime} : {message}";
        switch (logLevel)
        {
            case LogLevel.Debug:
                UnityEngine.Debug.Log(message);
                break;

            case LogLevel.Info:
                UnityEngine.Debug.LogFormat(LogType.Log, LogOption.None, context, "<color=#1769FD>{0}</color>",
                    message);
                break;

            case LogLevel.Warn:
                UnityEngine.Debug.LogWarning(message);
                break;

            case LogLevel.Error:
                UnityEngine.Debug.LogError(message);
                break;

            case LogLevel.NoLog:
                
                break;

            default:
                UnityEngine.Debug.LogFormat(LogType.Exception, LogOption.None, context,
                    "<color=#FF534A>{0}</color>", message);
                break;
        }
    }
}