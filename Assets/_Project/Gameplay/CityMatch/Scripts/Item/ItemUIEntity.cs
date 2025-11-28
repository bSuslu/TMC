using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemUIEntity : MonoBehaviour
    {
        public string ItemId{ get; private set; }
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _rectTransform;
        
        private Sequence _sequence;
        
        public void SetIDAndSprite(string itemId, Sprite sprite)
        {
            ItemId = itemId;
            _image.sprite = sprite;
        }

        public void MoveToSlotFromWorld(Vector2 slotViewRelativePosition, Vector2 slotSize, Action onFinishCallback = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_rectTransform.DOAnchorPos(slotViewRelativePosition, 0.25f).SetEase(Ease.InBack));
            _sequence.Join(_rectTransform.DOSizeDelta(slotSize, 0.3f));
            _sequence.OnComplete(() => { onFinishCallback?.Invoke(); });
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}