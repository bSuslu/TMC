using System.Collections.Generic;
using System.Linq;
using _Project.Core.Framework.EventBus;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMC._Project.Gameplay.CityMatch.Scripts.Events;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemMatchView : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [field: SerializeField] private RectTransform _itemUIEntityParent;
        [field: SerializeField] private ItemUIEntity _itemUIEntityPrefab;
        [field: SerializeField] private ItemMatchSlot[] _slots;

        private readonly int _matchCount = 3;
        private Vector2 _slotSize;
        private Vector2[] _relativeSlotPositions;
        private readonly List<ItemUIEntity> _items = new();
        private void Awake() => ItemEntity.OnItemClicked += OnItemEntityClicked;
        private void OnDestroy() => ItemEntity.OnItemClicked -= OnItemEntityClicked;
        

        private bool HasEmptySlot() => _items.Count < _slots.Length;

        private void OnItemEntityClicked(ItemEntity entity)
        {
            if (_relativeSlotPositions == null || _relativeSlotPositions.Length == 0) CalculateRelativeSlotPositions();

            if (!HasEmptySlot()) return;

            var itemUIEntity = Instantiate(_itemUIEntityPrefab, _itemUIEntityParent);
            itemUIEntity.SetIDAndSprite(entity.Id, entity.Config.Icon);
            itemUIEntity.transform.localPosition = GetRelativeLocalPointInRectangle(_itemUIEntityParent, entity.Position, _camera);

            var screenSize = GetItemUIScaleFromSprite(entity.Position, entity.SpriteRenderer);
            RectTransform rt = itemUIEntity.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Mathf.Abs(screenSize.x), Mathf.Abs(screenSize.y));

            int index = AddOrInsertItem(itemUIEntity);
            
            MoveUIItemToSlot(itemUIEntity, index).Forget();
            
            UpdateItemPositions(index);

            entity.ClickSuccess();
        }

        private async UniTask MoveUIItemToSlot(ItemUIEntity itemUIEntity, int index)
        {
            await itemUIEntity.MoveToSlotFromWorld(_relativeSlotPositions[index], _slotSize);
            await CheckMatchAsync();
            
            if (!HasEmptySlot())
            {
                EventBus<SlotsFullEvent>.Publish(new SlotsFullEvent());
            }
        }

        private int AddOrInsertItem(ItemUIEntity itemUIEntity)
        {
            int lastIndex = _items.FindLastIndex(x => x.ItemId == itemUIEntity.ItemId);
            if (lastIndex == -1)
            {
                _items.Add(itemUIEntity);
                return _items.Count - 1;
            }

            _items.Insert(lastIndex + 1, itemUIEntity);
            return lastIndex + 1;
        }

        private void UpdateItemPositions(int excludeIndex = -1)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (i == excludeIndex) continue;
                _items[i].UpdatePosition(_relativeSlotPositions[i]);
            }
        }

        private Vector2 GetRelativeLocalPointInRectangle(RectTransform referenceRectTransform, Vector3 worldPosition, Camera cam = null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(referenceRectTransform,
                RectTransformUtility.WorldToScreenPoint(cam, worldPosition), cam, out var localPoint);
            return localPoint;
        }
        
        private async UniTask CheckMatchAsync()
        {
            if (_items.Count < _matchCount) return;

            for (int i = 0; i <= _items.Count - _matchCount; i++)
            {
                bool allMatch = true;
                var baseId = _items[i].ItemId;
                
                for (int j = 0; j < _matchCount; j++)
                {
                    var item = _items[i + j];
                    if (item.ItemId != baseId || item.IsMatched)
                    {
                        allMatch = false;
                        break;
                    }
                }

                if (!allMatch)
                    continue;
                
                var matchItems = _items.GetRange(i, _matchCount);
                
                await MatchItemsAsync(i);

                foreach (var matchItem in matchItems)
                {
                    Destroy(matchItem.gameObject);
                    _items.Remove(matchItem);
                }
                
                UpdateItemPositions();
                return;
            }
        }

        private async UniTask MatchItemsAsync(int startIndex)
        {
            var items = _items.GetRange(startIndex, _matchCount);
            var center = GetCenterOfSlotPositions(startIndex, _matchCount);

            var tasks = items.Select(item => item.MoveForMatch(center)).ToArray();
            await UniTask.WhenAll(tasks);
        }

        private Vector2 GetCenterOfSlotPositions(int startIndex, int matchCount)
        {
            Vector2 center = Vector2.zero;
            for (int i = startIndex; i < startIndex + matchCount; i++)
            {
                center += _relativeSlotPositions[i];
            }
            return center / matchCount;
        }

        private Vector3 GetItemUIScaleFromSprite(Vector3 worldPosition, SpriteRenderer spriteRenderer)
        {
            float worldWidth = spriteRenderer.bounds.size.x;
            float worldHeight = spriteRenderer.bounds.size.y;
            Vector3 worldScale = new Vector3(worldWidth, worldHeight, 1f);
            Vector3 screenSize = _camera.WorldToScreenPoint((Vector3)worldPosition + worldScale) -
                                 _camera.WorldToScreenPoint(worldPosition);
            return screenSize;
        }

        private void CalculateRelativeSlotPositions()
        {
            _slotSize = _slots[0].GetComponent<RectTransform>().sizeDelta;
            _relativeSlotPositions = new Vector2[_slots.Length];
            for (int i = 0; i < _slots.Length; i++)
            {
                _relativeSlotPositions[i] = GetRelativeLocalPointInRectangle(_itemUIEntityParent, _slots[i].transform.position, _camera);
            }
        }
    }
}
