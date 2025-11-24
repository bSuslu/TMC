using _Project.Core.Framework.EventBus.Interfaces;

namespace _Project.Core.Systems.SceneSystem.Events
{
    public struct SceneTransitionStartedEvent : IEvent
    {
        public GameScene Scene { get; set; }

        public SceneTransitionStartedEvent(GameScene scene)
        {
            Scene = scene;
        }
    }
}