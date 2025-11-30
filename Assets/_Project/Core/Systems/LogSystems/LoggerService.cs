using UnityEngine;

namespace _Project.Core.Systems.LogSystems
{
    public class LoggerService
    {
        private readonly LoggerConfig _config;

        public LoggerService(LoggerConfig config)
        {
            _config = config;
        }

        public void Log(LogLevel level, string message)
        {
            if (!_config.EnableLogs)
                return;

            switch (level)
            {
                case LogLevel.Info:
                    if (_config.EnableInfo)
                        Debug.Log(message);
                    break;

                case LogLevel.Warning:
                    if (_config.EnableWarnings)
                        Debug.LogWarning(message);
                    break;

                case LogLevel.Error:
                    if (_config.EnableErrors)
                        Debug.LogError(message);
                    break;
            }
        }

        public void Info(string message)
            => Log(LogLevel.Info, message);

        public void Warning(string message)
            => Log(LogLevel.Warning, message);

        public void Error(string message)
            => Log(LogLevel.Error, message);
    }
}