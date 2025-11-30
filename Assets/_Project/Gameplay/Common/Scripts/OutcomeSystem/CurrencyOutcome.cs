using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.CurrencySystem.Datas;
using _Project.Core.Systems.CurrencySystem.Interfaces;
using _Project.Core.Systems.CurrencySystem.Settings;
using TMC._Project.Gameplay.Common.Scripts.UI;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.OutcomeSystem
{
    [CreateAssetMenu(fileName = "CurrencyOutcome", menuName = "SO/Outcome/Currency")]
    public class CurrencyOutcome : Outcome
    {
        public CurrencyType CurrencyType;
        public int Amount;

        public override void ApplyOutcome()
        {
            var currencyService = ServiceLocator.Global.Get<ICurrencyService>();

            if (Amount >= 0)
                currencyService.Add(CurrencyType, Amount);
            else
                currencyService.Spend(CurrencyType, -Amount);
        }

        public override void SetWidget(BaseWidget widget)
        {
            widget.Icon.sprite = ServiceLocator.Global.Get<CurrencySettings>().CurrencyConfigs[CurrencyType].Icon;
            widget.AmountText.text = Amount.ToString();
        }
    }
}