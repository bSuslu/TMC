using _Project.Core.Framework.EventBus.Interfaces;

namespace _Project.Core.Systems.SceneSystem.Events
{
    public struct LoadSceneRequestEvent : IEvent
    {
        public GameScene Scene { get; set; }
        public bool Reload { get; set; }

        public LoadSceneRequestEvent(GameScene scene, bool reload = false)
        {
            Scene = scene;
            Reload = reload;
        }
    }
}