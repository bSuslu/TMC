using System.Collections.Generic;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Framework.ServiceLocator.Bootstrappers;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using _Project.Core.Systems.CurrencySystem.Scripts;
using _Project.Core.Systems.CurrencySystem.Services;
using _Project.Core.Systems.LoadingSystem.Interfaces;
using _Project.Core.Systems.SaveSystem.Interfaces;
using _Project.Core.Systems.SaveSystem.Services;
using _Project.Core.Systems.SceneSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMC._Project.Gameplay.Common
{
    public class ServiceLocatorGlobalPersistentBootstrapper: ServiceLocatorGlobalBootstrapper
    {
        [SerializeField] private CurrencySettings _currencySettings;
        private readonly List<IAsyncService> _asyncServices = new();
        
        protected override void Bootstrap()
        {
            base.Bootstrap();
            
            _currencySettings.Initialize();
            ServiceLocator.Global.Register(_currencySettings);
            
            var save = new JsonSaveService();
            ServiceLocator.Global.Register<ISaveService>(save);
            RegisterIfAsync(save);

            var currency = new CurrencyService();
            ServiceLocator.Global.Register<ICurrencyService>(currency);
            RegisterIfAsync(currency);

            var scene = new SceneService();
            ServiceLocator.Global.Register(scene);
            RegisterIfAsync(scene);
        }

        private void RegisterIfAsync(object service)
        {
            if (service is IAsyncService asyncService)
                _asyncServices.Add(asyncService);
        }
        
        private async void Start()
        {
            foreach (var s in _asyncServices)
                await s.InitializeAsync();
        
            // loading tamamlandı
        }
        
        // private async void Start()
        // {
        //     var tasks = new List<UniTask>();
        //
        //     foreach (var service in _asyncServices)
        //         tasks.Add(service.InitializeAsync());
        //
        //     await UniTask.WhenAll(tasks);
        // }
    }
}