using System.Collections.Generic;
using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Framework.ServiceLocator.Bootstrappers;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using _Project.Core.Systems.CurrencySystem.Services;
using _Project.Core.Systems.CurrencySystem.Settings;
using _Project.Core.Systems.LoadingSystem.Events;
using _Project.Core.Systems.LoadingSystem.Interfaces;
using _Project.Core.Systems.SaveSystem.Interfaces;
using _Project.Core.Systems.SaveSystem.Services;
using _Project.Core.Systems.SceneSystem.Services;
using Cysharp.Threading.Tasks;
using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Service;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Settings;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.Bootstrappers
{
    public class ServiceLocatorPersistentBootstrapper: ServiceLocatorGlobalBootstrapper
    {
        [SerializeField] private CurrencySettings _currencySettings;
        [SerializeField] private LevelSettings _levelSettings;
        [SerializeField] private LivesSettings _livesSettings;
        [SerializeField] private ItemSettings _itemSettings;
        
        private readonly List<IAsyncService> _asyncServices = new();
        
        protected override void Bootstrap()
        {
            base.Bootstrap();
            
            _currencySettings.Initialize();
            ServiceLocator.Global.Register(_currencySettings);
            ServiceLocator.Global.Register(_levelSettings);
            ServiceLocator.Global.Register(_livesSettings);
            _itemSettings.Initialize();
            ServiceLocator.Global.Register(_itemSettings);
            
            var save = new JsonSaveService();
            ServiceLocator.Global.Register<ISaveService>(save);
            _asyncServices.Add(save);

            var levelService = new LevelService();
            ServiceLocator.Global.Register(levelService);
            _asyncServices.Add(levelService);
            
            var currency = new CurrencyService();
            ServiceLocator.Global.Register<ICurrencyService>(currency);
            _asyncServices.Add(currency);

            var liveService = new LivesService();
            ServiceLocator.Global.Register(liveService);
            _asyncServices.Add(liveService);

            var scene = new SceneService();
            ServiceLocator.Global.Register(scene);
            _asyncServices.Add(scene);
            
            InitializeAsync().Forget();
        }

        
        private async UniTask InitializeAsync()
        {
            foreach (var s in _asyncServices)
                await s.InitializeAsync();
            
            
            EventBus<ServicesReadyEvent>.Publish(new ServicesReadyEvent());
        }
    }
}