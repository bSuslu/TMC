using System.Collections.Generic;
using System.Linq;
using _Project.Core.Common.Bases;
using _Project.Core.Systems.CurrencySystem.Datas;
using UnityEngine;

namespace _Project.Core.Systems.CurrencySystem.Settings
{
    [CreateAssetMenu(fileName = "CurrencySettings", menuName = "SO/Currency/Settings")]
    public class CurrencySettings : BaseSettings
    {
        [SerializeField] private List<CurrencyConfig> _currencyConfigs;
        public Dictionary<CurrencyType, CurrencyConfig> CurrencyConfigs { get; private set; }

        public override void Initialize()
        {
            CurrencyConfigs = _currencyConfigs.ToDictionary(x => x.Type, x => x);
        }
    }
}