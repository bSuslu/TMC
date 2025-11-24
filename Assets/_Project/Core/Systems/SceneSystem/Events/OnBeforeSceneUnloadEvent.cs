using _Project.Core.Framework.EventBus.Interfaces;

namespace _Project.Core.Systems.SceneSystem.Events
{
    public struct OnBeforeSceneUnloadEvent : IEvent
    {
        public GameScene Scene { get; set; }

        public OnBeforeSceneUnloadEvent(GameScene scene)
        {
            Scene = scene;
        }
    }
}