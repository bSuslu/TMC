using _Project.Core.Framework.EventBus.Interfaces;
using _Project.Core.Systems.SceneSystem.SO;

namespace _Project.Core.Systems.SceneSystem.Events
{
    public struct OnBeforeSceneUnloadEvent : IEvent
    {
        public SceneType SceneType { get; set; }

        public OnBeforeSceneUnloadEvent(SceneType sceneType)
        {
            SceneType = sceneType;
        }
    }
}