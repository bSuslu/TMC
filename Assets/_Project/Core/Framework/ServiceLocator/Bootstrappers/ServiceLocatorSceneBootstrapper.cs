
namespace _Project.Core.Framework.ServiceLocator.Bootstrappers 
{
    public class ServiceLocatorSceneBootstrapper : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureForScene();
        }
    }
}