using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.CurrencySystem.Datas;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.RewardSystem
{
    [CreateAssetMenu(fileName = "CurrencyReward", menuName = "SO/Reward/CurrencyReward")]
    public class CurrencyReward : RewardBase
    {
        public CurrencyType CurrencyType;
        public int Amount;

        public override void ApplyReward(ServiceLocator sceneLocator)
        {
            var currencyService = ServiceLocator.Global.Get<ICurrencyService>();
            currencyService.Add(CurrencyType, Amount);
        }
    }
}
