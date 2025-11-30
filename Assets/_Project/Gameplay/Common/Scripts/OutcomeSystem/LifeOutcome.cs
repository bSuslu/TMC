using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Service;
using TMC._Project.Gameplay.Common.Scripts.UI;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.OutcomeSystem
{
    [CreateAssetMenu(fileName = "LifeOutcome", menuName = "SO/Outcome/Life")]
    public class LifeOutcome : Outcome
    {
        public int Amount;

        public override void ApplyOutcome()
        {
            var livesService = ServiceLocator.Global.Get<LivesService>();
            
            if (Amount < 0)
                livesService.RemoveLives(Amount);
            else
                livesService.AddLives(Amount);
        }

        public override void SetWidget(BaseWidget widget)
        {
            widget.AmountText.text = Amount.ToString();
        }
    }
}