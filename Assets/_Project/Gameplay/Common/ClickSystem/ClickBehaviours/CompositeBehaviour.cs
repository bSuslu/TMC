using UnityEngine;

namespace TMC._Project.Gameplay.Common.ClickSystem.ClickBehaviours
{
    [CreateAssetMenu(menuName = "Click Behaviours/Composite Behaviour")]
    public class CompositeBehaviour : ClickBehaviour
    {
        [SerializeField] private ClickBehaviour[] behaviours;
    
        public override void Execute(GameObject clickedObject)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.Execute(clickedObject);
            }
        }
    }
}