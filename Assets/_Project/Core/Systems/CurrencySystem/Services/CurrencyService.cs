using System;
using System.Collections.Generic;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.CurrencySystem.Datas;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using _Project.Core.Systems.CurrencySystem.Settings;
using _Project.Core.Systems.LogSystems;
using _Project.Core.Systems.SaveSystem.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Core.Systems.CurrencySystem.Services
{
    // CurrencyService manages player currencies, including loading, saving, updating, and unlocking.
    public class CurrencyService : ICurrencyService
    {
        public Dictionary<CurrencyType, CurrencyData> CurrencyDatas { get; private set; }
        public event Action<CurrencyType, int> OnCurrencyAmountUpdated;
        public event Action<CurrencyType> OnCurrencyUnlocked;

        private readonly CurrencySettings _currencySettings;
        private readonly ISaveService _saveService;
        private const string k_saveKey = "currency_data";

        // Constructor: resolves references to currency settings and save service via ServiceLocator.
        public CurrencyService()
        {
            _currencySettings = ServiceLocator.Global.Get<CurrencySettings>();
            _saveService = ServiceLocator.Global.Get<ISaveService>();
        }
        
        // Initializes the currency service, loading or creating currency data as needed.
        public async UniTask InitializeAsync()
        {
            Log.Info("Initializing CurrencyService");
            await InitializeCurrencyDatas();
            Log.Info("CurrencyService Initialized");
        }

        // Loads currency data from save, or initializes with default config if none found.
        private async UniTask InitializeCurrencyDatas()
        {
            var (success, loadedData) = await _saveService.TryLoadAsync<Dictionary<CurrencyType, CurrencyData>>(k_saveKey);
            
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

        // Initializes CurrencyDatas with default values from config.
        private void InitializeDefaultCurrencyDatas()
        {
            CurrencyDatas = new Dictionary<CurrencyType, CurrencyData>();
            foreach (var currencyConfigKvp in _currencySettings.CurrencyConfigs)
            {
                CurrencyDatas.Add(currencyConfigKvp.Key,
                    new CurrencyData(currencyConfigKvp.Value.InitialAmount));
            }
        }

        // Synchronizes loaded currency data with the current configuration (adds missing types, removes extras).
        // This is useful for live ops and backward compatibility.
        private void SyncDataWithConfig(out bool isDataChanged)
        {
            Log.Info("[CurrencyService] SyncDataWithConfig started");
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
                    CurrencyDatas[missingType] = new CurrencyData(config.InitialAmount);
                    Log.Warning($"[CurrencyService] Added missing currency type to data: {missingType}");
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
                    Log.Warning($"[CurrencyService] Delete currency types found in data: {extraType}");
                }
            }
            Log.Info("[CurrencyService] SyncDataWithConfig finished");
        }

        public bool HasEnoughCurrency(CurrencyType currencyType, int amountToCheck)
        {
            if (CurrencyDatas.TryGetValue(currencyType, out var currencyData))
            {
                return currencyData.Amount >= amountToCheck;
            }

            Log.Warning($"[CurrencyService] Currency type {currencyType} not found in CurrencyDatas.");
            return false;
        }

        public bool TrySpend(CurrencyType currencyType, int amountToCharge)
        {
            if (!HasEnoughCurrency(currencyType, amountToCharge)) return false;
            Log.Info($"[CurrencyService] TrySpend: {currencyType} -> {amountToCharge}");

            Spend(currencyType, amountToCharge);
            return true;
        }

        // Deducts the specified amount from the given currency type and saves the change.
        public void Spend(CurrencyType currencyType, int amountToCharge)
        {
            Log.Info($"[CurrencyService] Spend started: {currencyType}, amount: {amountToCharge}");
            if (!CurrencyDatas.TryGetValue(currencyType, out var data))
            {
                Log.Warning($"[CurrencyService] Spend failed: {currencyType} not found.");
                return;
            }

            data.Amount -= amountToCharge;
            Log.Info($"[CurrencyService] Spend completed: {currencyType}, new amount: {data.Amount}");
            OnCurrencyAmountUpdated?.Invoke(currencyType, data.Amount);
            SaveAsync().Forget();
        }

        // Adds the specified amount to the given currency type, unlocking if necessary.
        public void Add(CurrencyType currencyType, int amount)
        {
            if (!CurrencyDatas.TryGetValue(currencyType, out var data))
            {
                Log.Warning($"[CurrencyService] Add failed: {currencyType} not found.");
                return;
            }

            if (!data.IsUnlocked) Unlock(currencyType);

            data.Amount += amount;
            OnCurrencyAmountUpdated?.Invoke(currencyType, data.Amount);
            SaveAsync().Forget();
            Log.Info($"[CurrencyService] Add: {currencyType}, amount: {amount}, new total: {data.Amount}");
        }

        // Adds multiple currency rewards at once.
        public void Add(Dictionary<CurrencyType, int> rewards)
        {
            foreach (var reward in rewards)
            {
                Add(reward.Key, reward.Value);
            }
        }

        // Sets the amount for a currency type. Optionally saves the change.
        public void SetAmount(CurrencyType currencyType, int amount, bool doSave = true)
        {
            CurrencyDatas[currencyType].Amount = amount;
            Log.Info($"[CurrencyService] SetAmount: {currencyType} -> {amount}");
            OnCurrencyAmountUpdated?.Invoke(currencyType, CurrencyDatas[currencyType].Amount);
            if (doSave) SaveAsync().Forget();
        }

        // Unlocks a currency type, making it available to the player.
        public void Unlock(CurrencyType currencyType, bool doSave = true)
        {
            CurrencyDatas[currencyType].IsUnlocked = true;
            Log.Info($"[CurrencyService] Unlock: {currencyType}");
            OnCurrencyUnlocked?.Invoke(currencyType);
            if (doSave) SaveAsync().Forget();
        }

        // Gets the current amount of the specified currency type.
        public int GetAmount(CurrencyType currencyType)
        {
            if (CurrencyDatas.TryGetValue(currencyType, out CurrencyData currencyData))
            {
                return currencyData.Amount;
            }

            return 0;
        }

        // Saves the current currency data to persistent storage.
        private async UniTask SaveAsync()
        {
            await _saveService.SaveAsync(k_saveKey, CurrencyDatas);
        }

        // Returns the icon associated with the given currency type.
        public Sprite GetCurrencyIcon(CurrencyType currencyType)
        {
            return _currencySettings.CurrencyConfigs[currencyType].Icon;
        }

        // Resets all currencies to their default values from config.
        public void Reset()
        {
            InitializeDefaultCurrencyDatas();
            SaveAsync().Forget();
        }
    }
}