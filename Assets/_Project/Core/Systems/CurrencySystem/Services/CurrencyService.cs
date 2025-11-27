using System;
using System.Collections.Generic;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.CurrencySystem.Datas;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using _Project.Core.Systems.CurrencySystem.Scripts;
using _Project.Core.Systems.LoadingSystem.Interfaces;
using _Project.Core.Systems.SaveSystem.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace _Project.Core.Systems.CurrencySystem.Services
{
    public class CurrencyService : ICurrencyService, IAsyncService
    {
        public Dictionary<CurrencyType, CurrencyData> CurrencyDatas { get; private set; }
        public event Action<CurrencyType, int> OnCurrencyAmountUpdated;
        public event Action<CurrencyType> OnCurrencyUnlocked;

        private readonly CurrencySettings _currencySettings;
        private readonly ISaveService _saveService;
        private readonly string _currencyDataPath;

        public CurrencyService()
        {
            _currencySettings = ServiceLocator.Global.Get<CurrencySettings>();
            _currencyDataPath = _currencySettings.DataPath;
            _saveService = ServiceLocator.Global.Get<ISaveService>();
        }
        
        public async UniTask InitializeAsync()
        {
            Debug.Log("Initializing CurrencyService");
            await InitializeCurrencyDatas();
            Debug.Log("CurrencyService Initialized");
        }

        private async UniTask InitializeCurrencyDatas()
        {
            var (success, loadedData) = await _saveService.TryLoadAsync<Dictionary<CurrencyType, CurrencyData>>(_currencyDataPath);
            
            if (success)
            {
                CurrencyDatas = loadedData;

                SyncDataWithConfig(out bool isDataChanged);

                if (isDataChanged)
                {
                    await SaveAsync();
                }
                
            }
            else
            {
                InitializeDefaultCurrencyDatas();
                await SaveAsync();
            }
        }

        private void InitializeDefaultCurrencyDatas()
        {
            CurrencyDatas = new Dictionary<CurrencyType, CurrencyData>();
            foreach (var currencyConfigKvp in _currencySettings.CurrencyConfigs)
            {
                CurrencyDatas.Add(currencyConfigKvp.Key,
                    new CurrencyData(currencyConfigKvp.Value.InitialAmount));
            }
        }

        private void SyncDataWithConfig(out bool isDataChanged)
        {
            isDataChanged = false;
            var configKeys = new HashSet<CurrencyType>(_currencySettings.CurrencyConfigs.Keys);
            var dataKeys = new HashSet<CurrencyType>(CurrencyDatas.Keys);

            // Configde olup datada olmayan tipleri ekle
            var missingTypesInData = new List<CurrencyType>(configKeys);
            missingTypesInData.RemoveAll(k => dataKeys.Contains(k));

            if (missingTypesInData.Count > 0)
            {
                isDataChanged = true;
                foreach (var missingType in missingTypesInData)
                {
                    var config = _currencySettings.CurrencyConfigs[missingType];
                    CurrencyDatas[missingType] = new CurrencyData(0);
                    Debug.LogWarning($"[CurrencyService] Added missing currency type to data: {missingType}");
                }
            }

            // Fazla tipler (datada(json içinde) var, configte(editörde) yok)
            var extraTypesInData = new List<CurrencyType>(dataKeys);
            extraTypesInData.RemoveAll(k => configKeys.Contains(k));

            if (extraTypesInData.Count > 0)
            {
                isDataChanged = true;
                foreach (var extraType in extraTypesInData)
                {
                    CurrencyDatas.Remove(extraType);
                    Debug.LogWarning($"[CurrencyService] Delete currency types found in data: {extraType}");
                }
            }
        }

        public bool HasEnoughCurrency(CurrencyType currencyType, int amountToCheck)
        {
            if (CurrencyDatas.TryGetValue(currencyType, out var currencyData))
            {
                return currencyData.Amount >= amountToCheck;
            }

            Debug.LogWarning($"[CurrencyService] Currency type {currencyType} not found in CurrencyDatas.");
            return false;
        }

        public bool TrySpend(CurrencyType currencyType, int amountToCharge)
        {
            if (!HasEnoughCurrency(currencyType, amountToCharge)) return false;

            Spend(currencyType, amountToCharge);
            return true;
        }

        public void Spend(CurrencyType currencyType, int amountToCharge)
        {
            CurrencyDatas[currencyType].Amount -= amountToCharge;
            OnCurrencyAmountUpdated?.Invoke(currencyType, CurrencyDatas[currencyType].Amount);
            SaveAsync().Forget();
        }

        // Method to reward currency
        public void Add(CurrencyType currencyType, int amount)
        {
            if (!CurrencyDatas[currencyType].IsUnlocked) Unlock(currencyType);
            CurrencyDatas[currencyType].Amount += amount;
            OnCurrencyAmountUpdated?.Invoke(currencyType, CurrencyDatas[currencyType].Amount);
            SaveAsync().Forget();
        }

        public void Add(Dictionary<CurrencyType, int> rewards)
        {
            foreach (var reward in rewards)
            {
                Add(reward.Key, reward.Value);
            }
        }

        public void SetAmount(CurrencyType currencyType, int amount, bool doSave = true)
        {
            CurrencyDatas[currencyType].Amount = amount;
            OnCurrencyAmountUpdated?.Invoke(currencyType, CurrencyDatas[currencyType].Amount);
            if (doSave) SaveAsync().Forget();
        }

        public void Unlock(CurrencyType currencyType, bool doSave = true)
        {
            CurrencyDatas[currencyType].IsUnlocked = true;
            OnCurrencyUnlocked?.Invoke(currencyType);
            if (doSave) SaveAsync().Forget();
        }

        public int GetAmount(CurrencyType currencyType)
        {
            if (CurrencyDatas.TryGetValue(currencyType, out CurrencyData currencyData))
            {
                return currencyData.Amount;
            }

            return 0;
        }

        private async UniTask SaveAsync()
        {
            await _saveService.SaveAsync(_currencyDataPath, CurrencyDatas);
        }

        public Sprite GetCurrencyIcon(CurrencyType currencyType)
        {
            return _currencySettings.CurrencyConfigs[currencyType].Icon;
        }

        public void Reset()
        {
            InitializeDefaultCurrencyDatas();
            SaveAsync().Forget();
        }
    }
}