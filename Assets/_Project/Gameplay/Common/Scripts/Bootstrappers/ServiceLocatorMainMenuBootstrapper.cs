using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.ServiceLocator.Bootstrappers;
using _Project.Core.Systems.LoadingSystem.Events;

namespace TMC._Project.Gameplay.Common.Scripts.Bootstrappers
{
    public class ServiceLocatorMainMenuBootstrapper : ServiceLocatorSceneBootstrapper
    {
        protected override void Bootstrap()
        {
            base.Bootstrap();
            
            EventBus<ServicesReadyEvent>.Publish(new ServicesReadyEvent());
        }
    }
}