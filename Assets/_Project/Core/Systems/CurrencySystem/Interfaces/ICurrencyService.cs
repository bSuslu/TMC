using System;
using System.Collections.Generic;
using _Project.Core.Systems.CurrencySystem.Datas;
using _Project.Core.Systems.LoadingSystem.Interfaces;
using UnityEngine;

namespace _Project.Core.Systems.CurrencySystem.Interfaces
{
    public interface ICurrencyService: IAsyncService
    {
        public Dictionary<CurrencyType, CurrencyData> CurrencyDatas { get; }
        public event Action<CurrencyType, int> OnCurrencyAmountUpdated;
        public event Action<CurrencyType> OnCurrencyUnlocked;
        public bool HasEnoughCurrency(CurrencyType currencyType, int amountToCheck);
        public bool TrySpend(CurrencyType currencyType, int amountToCharge);
        public void Spend(CurrencyType currencyType, int amountToCharge);
        public void Add(CurrencyType currencyType, int amount);
        public void Add(Dictionary<CurrencyType, int> rewards);
        public void SetAmount(CurrencyType currencyType, int amount, bool doSave = true);
        public int GetAmount(CurrencyType currencyType);
        public Sprite GetCurrencyIcon(CurrencyType currencyType);
        public void Unlock(CurrencyType currencyType, bool doSave = true);
    }
}