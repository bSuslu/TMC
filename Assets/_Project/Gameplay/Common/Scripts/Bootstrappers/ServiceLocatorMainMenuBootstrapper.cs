using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Framework.ServiceLocator.Bootstrappers;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;

namespace TMC._Project.Gameplay.Common.Scripts.Bootstrappers
{
    public class ServiceLocatorMainMenuBootstrapper : ServiceLocatorSceneBootstrapper
    {
        protected override void Bootstrap()
        {
            base.Bootstrap();
            
            LevelService levelService = ServiceLocator.Global.Get<LevelService>();
        }
    }
}