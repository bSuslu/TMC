using _Project.Core.Framework.EventBus.Interfaces;

namespace _Project.Core.Systems.SceneSystem.Events
{
    public struct SceneTransitionCompletedEvent : IEvent
    {
        public GameScene Scene { get; set; }

        public SceneTransitionCompletedEvent(GameScene scene)
        {
            Scene = scene;
        }
    }
}