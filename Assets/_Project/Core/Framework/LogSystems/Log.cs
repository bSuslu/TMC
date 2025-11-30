namespace _Project.Core.Framework.LogSystems
{
    public static class Log
    {
        private static LoggerService _logger;

        public static void Initialize(LoggerService logger)
        {
            _logger = logger;
        }

        public static void Info(string msg)
        {
            _logger.Info(msg);
        }

        public static void Warning(string msg)
        {
            _logger.Warning(msg);
        }

        public static void Error(string msg)
        {
            _logger.Error(msg);
        }
    }
}