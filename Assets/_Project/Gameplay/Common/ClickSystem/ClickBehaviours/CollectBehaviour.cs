using UnityEngine;

namespace TMC._Project.Gameplay.Common.ClickSystem.ClickBehaviours
{
    [CreateAssetMenu(menuName = "Click Behaviours/Collect Behaviour")]
    public class CollectBehaviour : ClickBehaviour
    {
        // [SerializeField] private CollectionSlotManager slotManager;
    
        public override void Execute(GameObject clickedObject)
        {
            // Toplama mantığı burada
            // slotManager.AddToFirstEmptySlot(clickedObject);
            // CheckForMatches();
        }
    }
}