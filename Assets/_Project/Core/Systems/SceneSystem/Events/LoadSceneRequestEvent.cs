using _Project.Core.Framework.EventBus.Interfaces;
using _Project.Core.Systems.SceneSystem.SO;

namespace _Project.Core.Systems.SceneSystem.Events
{
    public struct LoadSceneRequestEvent : IEvent
    {
        public SceneType SceneType { get; set; }
        public bool Reload { get; set; }

        public LoadSceneRequestEvent(SceneType sceneType, bool reload = false)
        {
            SceneType = sceneType;
            Reload = reload;
        }
    }
}