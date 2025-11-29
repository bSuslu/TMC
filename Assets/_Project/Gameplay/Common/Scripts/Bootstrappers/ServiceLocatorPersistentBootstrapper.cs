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
using TMC._Project.Gameplay.CityMatch.Scripts.Level;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.Bootstrappers
{
    public class ServiceLocatorPersistentBootstrapper: ServiceLocatorGlobalBootstrapper
    {
        [SerializeField] private CurrencySettings _currencySettings;
        [SerializeField] private LevelSettings _levelSettings;
        
        private readonly List<IAsyncService> _asyncServices = new();
        
        protected override void Bootstrap()
        {
            base.Bootstrap();
            
            _currencySettings.Initialize();
            ServiceLocator.Global.Register(_currencySettings);
            ServiceLocator.Global.Register(_levelSettings);
            
            var save = new JsonSaveService();
            ServiceLocator.Global.Register<ISaveService>(save);
            _asyncServices.Add(save);

            var levelService = new LevelService();
            ServiceLocator.Global.Register(levelService);
            _asyncServices.Add(levelService);
            
            var currency = new CurrencyService();
            ServiceLocator.Global.Register<ICurrencyService>(currency);
            _asyncServices.Add(currency);

            var scene = new SceneService();
            ServiceLocator.Global.Register(scene);
            _asyncServices.Add(scene);
            
            InitializeAsync().Forget();
        }

        
        private async UniTask InitializeAsync()
        {
            foreach (var s in _asyncServices)
                await s.InitializeAsync();
            
            // fire event
            EventBus<ServicesReadyEvent>.Publish(new ServicesReadyEvent());
        }
    }
}