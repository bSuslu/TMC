using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.CurrencySystem.Datas;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.OutcomeActionSystem
{
    [CreateAssetMenu(fileName = "CurrencyCost", menuName = "SO/Outcome/Cost/CurrencyCost")]
    public class CurrencyCostOutcome :OutcomeAction
    {
        public CurrencyType CurrencyType;
        public int Amount;

        public override void ApplyReward(ServiceLocator sceneLocator)
        {
            var currencyService = ServiceLocator.Global.Get<ICurrencyService>();
            currencyService.Spend(CurrencyType, Amount);
        }
    }
}