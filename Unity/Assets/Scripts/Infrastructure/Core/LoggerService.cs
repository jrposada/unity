namespace Infrastructure
{
    public enum LoggerLevel
    {
        All,
        Debug,
        Info,
        Warn,
        Error,
        Develop
    }

    /// <summary>
    /// Interface to log message to differen log levels.
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Log to develop level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="memberName">Caller class name. (Filled by compiler)</param>
        void LogDevelop(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "");

        /// <summary>
        /// Log debug level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="memberName">Caller class name. (Filled by compiler)</param>
        void LogDebug(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "");

        /// <summary>
        /// Log to info level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="memberName">Caller class name. (Filled by compiler)</param>
        void LogInfo(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "");

        /// <summary>
        /// Log to warning level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="memberName">Caller class name. (Filled by compiler)</param>
        void LogWarning(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "");

        /// <summary>
        /// Log to error level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="memberName">Caller class name. (Filled by compiler)</param>
        void LogError(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "");
    }

    /// <inheritdoc cref="ILoggerService"/>
    /// <summary>
    /// Proxy to Unity's default logger.
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private readonly LoggerLevel m_loggerLevel = LoggerLevel.All;

        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        /// <param name="loggerLevel">Max level to log.</param>
        public LoggerService(LoggerLevel loggerLevel)
        {
            m_loggerLevel = loggerLevel;
        }

        /// <inheritdoc cref="ILoggerService.LogDevelop(string, string)"/>
        public void LogDevelop(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (m_loggerLevel == LoggerLevel.Develop)
                UnityEngine.Debug.Log($"[{memberName}] DEVELOP: {message}.");
        }

        /// <inheritdoc cref="ILoggerService.LogDebug(string, string)"/>
        public void LogDebug(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (m_loggerLevel <= LoggerLevel.Info)
                UnityEngine.Debug.Log($"[{memberName}] DEBUG: {message}.");
        }

        /// <inheritdoc cref="ILoggerService.LogInfo(string, string)"/>
        public void LogInfo(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (m_loggerLevel <= LoggerLevel.Info)
                UnityEngine.Debug.Log($"[{memberName}] INFO: {message}.");
        }

        /// <inheritdoc cref="ILoggerService.LogWarning(string, string)"/>
        public void LogWarning(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (m_loggerLevel <= LoggerLevel.Warn)
                UnityEngine.Debug.Log($"[{memberName}] WARN: {message}.");
        }

        /// <inheritdoc cref="ILoggerService.LogError(string, string)"/>
        public void LogError(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (m_loggerLevel <= LoggerLevel.Error)
                UnityEngine.Debug.Log($"[{memberName}] ERROR: {message}.");
        }
    }
}
