using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Service;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.RewardSystem
{
    [CreateAssetMenu(fileName = "LifeReward", menuName = "SO/Reward/LifeReward")]
    public class LifeReward : RewardBase
    {
        public int Amount;

        public override void ApplyReward(ServiceLocator sceneLocator)
        {
            var livesService = ServiceLocator.Global.Get<LivesService>();
            livesService.AddLives(Amount);
        }
    }
}