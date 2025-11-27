using DG.Tweening;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    public class ItemEntity : MonoBehaviour, IClickable
    {
        [SerializeField] private ItemMatchView _itemMatchView;
        public int ItemId;
        
        public void OnClick()
        {
            if (_itemMatchView.TryGetFirstEmptySlot(out var slot))
            {
                transform.DOMove(slot.transform.position, 0.5f);
            }
        }
    }
}