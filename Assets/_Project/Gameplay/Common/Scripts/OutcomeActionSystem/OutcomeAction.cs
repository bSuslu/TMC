using _Project.Core.Framework.ServiceLocator;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.OutcomeActionSystem
{
    public abstract class OutcomeAction : ScriptableObject
    {
        public abstract void ApplyReward(ServiceLocator sceneLocator);
    }
}