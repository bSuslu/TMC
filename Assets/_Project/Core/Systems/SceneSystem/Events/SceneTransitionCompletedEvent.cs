using _Project.Core.Framework.EventBus.Interfaces;

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