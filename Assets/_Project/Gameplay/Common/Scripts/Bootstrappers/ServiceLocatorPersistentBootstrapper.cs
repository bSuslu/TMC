using System.Collections.Generic;
using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.LogSystems;
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
        
        [SerializeField] private LoggerConfig _loggerConfig;

        private readonly List<IAsyncService> _asyncServices = new();
        
        
        protected override void Bootstrap()
        {
            Application.targetFrameRate = 60;
            
            base.Bootstrap();
            
            var global = ServiceLocator.Global;
            
            var loggerService = new LoggerService(_loggerConfig);
            // for static facade
            Log.Initialize(loggerService);
            
            global.Register(_currencySettings).Initialize();
            global.Register(_levelSettings).Initialize();
            global.Register(_livesSettings).Initialize();
            global.Register(_itemSettings).Initialize();
            
            _asyncServices.Add(global.Register<ISaveService>(new JsonSaveService()));
            _asyncServices.Add(global.Register(new LevelService()));
            _asyncServices.Add(global.Register<ICurrencyService>(new CurrencyService()));
            _asyncServices.Add(global.Register(new LivesService()));
            _asyncServices.Add(global.Register(new SceneService()));
            
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