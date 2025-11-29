using _Project.Core.Framework.EventBus.Interfaces;
using _Project.Core.Systems.SceneSystem.SO;

namespace _Project.Core.Systems.SceneSystem.Events
{
    public struct SceneTransitionStartedEvent : IEvent
    {
        public SceneType SceneType { get; set; }

        public SceneTransitionStartedEvent(SceneType sceneType)
        {
            SceneType = sceneType;
        }
    }
}