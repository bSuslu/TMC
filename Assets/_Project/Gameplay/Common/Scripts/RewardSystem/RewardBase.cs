using _Project.Core.Framework.ServiceLocator;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.RewardSystem
{
    public abstract class RewardBase : ScriptableObject
    {
        public abstract void ApplyReward(ServiceLocator sceneLocator);
    }
}