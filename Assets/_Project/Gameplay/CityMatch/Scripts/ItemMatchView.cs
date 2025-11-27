using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    public class ItemMatchView : MonoBehaviour
    {
        [field: SerializeField] public ItemMatchSlotView[] Slots { get; private set; }

        public bool TryGetFirstEmptySlot(out ItemMatchSlotView itemMatchSlotView)
        {
            itemMatchSlotView = null;
            foreach (var slot in Slots)
            {
                // TODO: Better way to null check
                if (slot.ItemImage.sprite == null)
                {
                    itemMatchSlotView = slot;
                    return true;
                }
            }
            return false;
        }
        
        public Vector3 GetSlotPosition(int index) => Slots[index].transform.position;
    }
}
