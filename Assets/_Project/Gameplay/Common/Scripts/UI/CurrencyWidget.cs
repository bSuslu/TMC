using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.CurrencySystem.Datas;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using _Project.Core.Systems.CurrencySystem.Settings;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.UI
{
    public class CurrencyWidget : BaseWidget 
    {
        [SerializeField] private CurrencyType _currencyType;
        
        private ICurrencyService _currencyService;
        private CurrencySettings _currencySettings;

        private void Awake()
        {
            _currencyService = ServiceLocator.Global.Get<ICurrencyService>();
            _currencySettings = ServiceLocator.Global.Get<CurrencySettings>();

            if (_currencySettings.CurrencyConfigs.TryGetValue(_currencyType, out var currencyConfig))
            {
                Icon.sprite = currencyConfig.Icon;
                AmountText.text = _currencyService.GetAmount(_currencyType).ToString();
            }
            else
            {
                Debug.LogError($"Currency config not found for currency type: {_currencyType}");
                
                _currencyService.OnCurrencyAmountUpdated += OnCurrencyAmountUpdated;
            }
        }

        private void OnDestroy()
        {
            _currencyService.OnCurrencyAmountUpdated -= OnCurrencyAmountUpdated;
        }

        private void OnCurrencyAmountUpdated(CurrencyType currencyType, int amount)
        {
            if (currencyType != _currencyType) return;
            
            AmountText.text = amount.ToString();
        }
    }
}
