using NLog;
using System;

namespace TweetBook.Utilities
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/net-core-web-api-logging-using-nlog-in-text-file/
    /// </summary>
    public class LoggerService : ILog
    {
        private readonly static ILogger logger = LogManager.GetCurrentClassLogger();

        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public void LogError(string message)
        {
            logger.Error(message);
        }

        public void LogInfo(string message)
        {
            logger.Info(message);
        }

        public void LogWarn(string message)
        {
            logger.Warn(message);
        }
    }
}
