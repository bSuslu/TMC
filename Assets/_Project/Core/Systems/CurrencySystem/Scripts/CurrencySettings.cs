using System.Collections.Generic;
using System.Linq;
using _Project.Core.Systems.CurrencySystem.Datas;
using UnityEngine;

namespace _Project.Core.Systems.CurrencySystem.Scripts
{
    [CreateAssetMenu(fileName = "CurrencySettings", menuName = "Settings/CurrencySettings")]
    public class CurrencySettings : ScriptableObject
    {
        [SerializeField] private List<CurrencyConfig> _currencyConfigs;
        public Dictionary<CurrencyType, CurrencyConfig> CurrencyConfigs { get; private set; }
        
        public void Initialize()
        {
            CurrencyConfigs = _currencyConfigs.ToDictionary(x => x.Type, x => x);
        }
    }
}