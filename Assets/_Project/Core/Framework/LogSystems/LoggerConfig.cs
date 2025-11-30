using UnityEngine;

namespace _Project.Core.Framework.LogSystems
{
    [CreateAssetMenu(fileName = "LoggerConfig", menuName = "SO/Logger/Config")]
    public class LoggerConfig : ScriptableObject
    {
        public bool EnableLogs = true;
        public bool EnableInfo = true;
        public bool EnableWarnings = true;
        public bool EnableErrors = true;
    }
}