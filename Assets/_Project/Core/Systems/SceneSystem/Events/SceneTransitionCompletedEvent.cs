using _Project.Core.Framework.EventBus.Interfaces;
using _Project.Core.Systems.SceneSystem.SO;

namespace _Project.Core.Systems.SceneSystem.Events
{
    public struct SceneTransitionCompletedEvent : IEvent
    {
        public SceneType SceneType { get; set; }

        public SceneTransitionCompletedEvent(SceneType sceneType)
        {
            SceneType = sceneType;
        }
    }
}