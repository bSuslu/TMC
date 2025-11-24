namespace _Project.Core.Framework.ServiceLocator.Bootstrappers
{
    public class  ServiceLocatorGlobalBootstrapper: Bootstrapper
    {

        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal(false);
        }
    }
}
