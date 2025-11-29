using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.LivesSystem.Behaviours.Base
{
    public abstract class LivesFinishedAction :ScriptableObject
    {
        public abstract void Execute();
    }
}