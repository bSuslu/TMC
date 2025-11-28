using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemMatchSlot : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        public bool IsEmpty => CurrentItemUIEntity == null;
        public ItemUIEntity CurrentItemUIEntity { get; set; }
        public string ItemID => CurrentItemUIEntity.ItemId;
    }
}