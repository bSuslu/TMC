using UnityEngine;

namespace TMC._Project.Gameplay.Common.ClickSystem.ClickBehaviours
{
    public abstract class ClickBehaviour : ScriptableObject
    {
        public abstract void Execute(GameObject clickedObject);
    }
}