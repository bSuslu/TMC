using UnityEngine;

namespace TMC._Project.Gameplay.Common.ClickSystem.ClickBehaviours
{
    public class ClickableObject : MonoBehaviour, IClickable
    {
        // [SerializeField] private ClickBehaviour _clickBehaviour;
        private ClickBehaviour _clickBehaviour;

        public void HandleClick()
        {
            // _clickBehaviour.Execute(this);
        }

        public void SetClickBehaviour(ClickBehaviour newBehaviour)
        {
            _clickBehaviour = newBehaviour;
        }
    }
}