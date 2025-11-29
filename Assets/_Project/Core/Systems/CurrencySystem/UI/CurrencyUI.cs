using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.CurrencySystem.Datas;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using _Project.Core.Systems.CurrencySystem.Scripts;
using UnityEngine;

namespace _Project.Core.Systems.CurrencySystem.UI
{
    public class CurrencyUI : MonoBehaviour
    {
        // [SerializeField] private CurrencyUIItem _currencyUIItemPrefab;
        // private readonly Dictionary<CurrencyType, CurrencyUIItem> _currencyViews = new ();
        private ICurrencyService _currencyService;
        private CurrencySettings _currencySettings;
        [SerializeField] private Transform _currencyUIItemsContainer;
        
        private void Awake()
        {
            _currencyService = ServiceLocator.Global.Get<ICurrencyService>();
            _currencySettings = ServiceLocator.Global.Get<CurrencySettings>();
            
            _currencyService.OnCurrencyAmountUpdated += OnCurrencyAmountUpdated;
            _currencyService.OnCurrencyUnlocked += OnCurrencyUnlocked;
        }

        private void OnCurrencyUnlocked(CurrencyType currencyType)
        {
            CreateCurrencyView(currencyType);
        }

        private void OnDestroy()
        {
            _currencyService.OnCurrencyAmountUpdated -= OnCurrencyAmountUpdated;
            _currencyService.OnCurrencyUnlocked -= OnCurrencyUnlocked;
        }
        
        private void Start()
        {
            CreateCurrencyViews();
        }

        private void CreateCurrencyViews()
        {
            foreach (var currencyData in _currencyService.CurrencyDatas)
            {
                if (currencyData.Value.IsUnlocked)
                {
                    CreateCurrencyView(currencyData.Key);
                }
            }
        }

        private void CreateCurrencyView(CurrencyType currencyType)
        {
            // var currencyView = Object.Instantiate(_currencyUIItemPrefab, _currencyUIItemsContainer);
            // currencyView.SetCurrencyAmount(_currencyService.GetAmount(currencyType));
            // _currencyViews.Add(currencyType, currencyView);
        }
        
        private void OnCurrencyAmountUpdated(CurrencyType currencyType, int amount)
        {
            // _currencyViews[currencyType].SetCurrencyAmount(amount);
        }
    }
}