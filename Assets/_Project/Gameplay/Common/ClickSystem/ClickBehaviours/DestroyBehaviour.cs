using UnityEngine;

namespace TMC._Project.Gameplay.Common.ClickSystem.ClickBehaviours
{
    [CreateAssetMenu(menuName = "Click Behaviours/Destroy Behaviour")]
    public class DestroyBehaviour : ClickBehaviour
    {
        [SerializeField] private float destroyDelay = 0f;
    
        public override void Execute(GameObject clickedObject)
        {
            Destroy(clickedObject, destroyDelay);
        }
    }
}