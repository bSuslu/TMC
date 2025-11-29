using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.CurrencySystem.Datas;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using _Project.Core.Systems.CurrencySystem.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Core.Systems.CurrencySystem.UI
{
    public class CurrencyWidget : MonoBehaviour
    {
        [SerializeField] private CurrencyType _currencyType;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;
        
        private ICurrencyService _currencyService;
        private CurrencySettings _currencySettings;

        private void Awake()
        {
            _currencyService = ServiceLocator.Global.Get<ICurrencyService>();
            _currencySettings = ServiceLocator.Global.Get<CurrencySettings>();

            if (_currencySettings.CurrencyConfigs.TryGetValue(_currencyType, out var currencyConfig))
            {
                _icon.sprite = currencyConfig.Icon;
                _amountText.text = _currencyService.GetAmount(_currencyType).ToString();
            }
            else
            {
                Debug.LogError($"Currency config not found for currency type: {_currencyType}");
            }
        }

        private void Start()
        {
            _currencyService.OnCurrencyAmountUpdated += OnCurrencyAmountUpdated;
        }

        private void OnDestroy()
        {
            _currencyService.OnCurrencyAmountUpdated -= OnCurrencyAmountUpdated;
        }

        private void OnCurrencyAmountUpdated(CurrencyType currencyType, int amount)
        {
            if (currencyType != _currencyType) return;
            
            _amountText.text = amount.ToString();
        }
    }
}
