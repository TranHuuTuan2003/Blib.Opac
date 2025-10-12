using Microsoft.Extensions.Logging;

namespace KMS.Shared.Helpers
{
    public static class LoggerHelper
    {
        public static void LogInformation(ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }

        public static void LogWarning(ILogger logger, string message, params object[] args)
        {
            logger.LogWarning(message, args);
        }

        public static void LogError(ILogger logger, Exception ex, string message, params object[] args)
        {
            logger.LogError(ex, message, args);
        }

        public static void LogDebug(ILogger logger, string message, params object[] args)
        {
            logger.LogDebug(message, args);
        }

        public static void LogQuery(bool enableQueryLog, ILogger logger, string message, params object[] args)
        {
            if (enableQueryLog)
            {
                logger.LogInformation("[QUERY] " + message, args);
            }
        }
    }
}
