using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemUIEntity : MonoBehaviour
    {
        public bool IsMatched { get; private set; }
        public string ItemId{ get; private set; }
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _rectTransform;
        
        
        public void SetIDAndSprite(string itemId, Sprite sprite)
        {
            IsMatched = false;
            ItemId = itemId;
            _image.sprite = sprite;
        }

        public async UniTask MoveToSlotFromWorld(Vector2 slotViewRelativePosition, Vector2 slotSize, Action onFinishCallback = null)
        {
            var move = _rectTransform.DOAnchorPos(slotViewRelativePosition, .5f).SetEase(Ease.InOutBack);
            var size = _rectTransform.DOSizeDelta(slotSize, .5f).SetEase(Ease.Linear);
            await UniTask.WhenAll(move.ToUniTask(), size.ToUniTask());
            onFinishCallback?.Invoke();
        }

        public void UpdatePosition(Vector2 slotViewRelativePosition)
        {
            if(IsMatched)return;
            _rectTransform.DOAnchorPos(slotViewRelativePosition, .5f).SetEase(Ease.InOutBack);
        }

        public async UniTask MoveForMatch(Vector2 matchPosition)
        {
            IsMatched = true;
            await _rectTransform.DOAnchorPos(matchPosition, .5f).SetEase(Ease.InOutBack);
        }
    }
}