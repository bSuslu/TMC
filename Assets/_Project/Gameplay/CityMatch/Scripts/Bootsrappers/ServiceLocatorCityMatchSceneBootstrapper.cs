using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Framework.ServiceLocator.Bootstrappers;
using _Project.Core.Systems.LoadingSystem.Events;
using _Project.Core.Systems.TimeSystem.Interfaces;
using _Project.Core.Systems.TimeSystem.Services;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Bootsrappers
{
    public class ServiceLocatorCityMatchSceneBootstrapper: ServiceLocatorSceneBootstrapper
    {
        protected override void Bootstrap()
        {
            base.Bootstrap();
            
            LevelService levelService = ServiceLocator.Global.Get<LevelService>();
            float levelDuration = levelService.ActiveLevelConfig.DurationInSeconds;
                
            var gameTimerService = new GameTimerService();
            ServiceLocator.ForSceneOf(this).Register<IGameTimerService>(gameTimerService);
            gameTimerService.StartTimerAsync(levelDuration);
            
            EventBus<ServicesReadyEvent>.Publish(new ServicesReadyEvent());
        }
    }
}