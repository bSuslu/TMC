using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Service;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.OutcomeActionSystem
{
    [CreateAssetMenu(fileName = "LifeCost", menuName = "SO/Outcome/Cost/LifeCost")]
    public class LifeCostOutcome:OutcomeAction
    {
        public int Amount;
        public override void ApplyReward(ServiceLocator sceneLocator)
        {
            var livesService = ServiceLocator.Global.Get<LivesService>();
            livesService.RemoveLives(Amount);
        }
    }
}