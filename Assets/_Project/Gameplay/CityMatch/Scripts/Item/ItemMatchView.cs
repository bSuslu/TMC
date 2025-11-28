using System.Linq;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemMatchView : MonoBehaviour
    {
        [field: SerializeField] private RectTransform _itemUIEntityParent;
        [field: SerializeField] private Transform[] _slotsTransforms;
        [field: SerializeField] private ItemUIEntity _itemUIEntityPrefab;

        private Camera _camera;
        private ItemMatchSlot[] _slots;
        private Vector2 _slotSize;

        private void Awake()
        {
            ItemEntity.OnItemClicked += ItemUIEntity_OnItemClicked;
        }

        private void CreateAndInitializeSlots()
        {
            _camera = Camera.main;
            _slotSize = _slotsTransforms[0].GetComponent<RectTransform>().sizeDelta;
            _slots = new ItemMatchSlot[_slotsTransforms.Length];
            for (int i = 0; i < _slotsTransforms.Length; i++)
            {
                var relativePosition =
                    GetRelativeLocalPointInRectangle(_itemUIEntityParent, _slotsTransforms[i].position, _camera);
                _slots[i] = new ItemMatchSlot(relativePosition);
            }
        }

        private void ItemUIEntity_OnItemClicked(ItemEntity entity)
        {
            if (_slots == null)
                CreateAndInitializeSlots();

            if (!HasEmptySlot())
                return;

            var slot = GetProperSlot(entity.Config.Id);

            var itemUIEntity = Instantiate(_itemUIEntityPrefab, _itemUIEntityParent);
            itemUIEntity.SetIDAndSprite(entity.Config.Id, entity.Config.Sprite);
            slot.CurrentItemUIEntity = itemUIEntity;

            itemUIEntity.transform.localPosition =
                GetRelativeLocalPointInRectangle(_itemUIEntityParent, entity.Position, _camera);

            var screenSize = GetItemUIScaleFromSprite(entity.Position, entity.SpriteRenderer);
            RectTransform rt = itemUIEntity.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Mathf.Abs(screenSize.x), Mathf.Abs(screenSize.y));

            itemUIEntity.MoveToSlotFromWorld(slot.RelativePosition, _slotSize, MovementComplete);

            entity.ClickSuccess();
        }

        private Vector2 GetRelativeLocalPointInRectangle(RectTransform referenceRectTransform, Vector3 worldPosition,
            Camera cam = null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(referenceRectTransform,
                RectTransformUtility.WorldToScreenPoint(cam, worldPosition), cam, out var localPoint);
            return localPoint;
        }

        private void MovementComplete()
        {
            CheckTripleMerge();
        }

        private void CheckTripleMerge()
        {
            for (int i = 2; i < _slots.Length; i++)
            {
                if (_slots[i].CurrentItemUIEntity == null) break;

                if (_slots[i].ItemID == _slots[i - 1].ItemID && _slots[i - 1].ItemID == _slots[i - 2].ItemID)
                {
                    // Triple merge found
                    Destroy(_slots[i].CurrentItemUIEntity.gameObject);
                    Destroy(_slots[i - 1].CurrentItemUIEntity.gameObject);
                    Destroy(_slots[i - 2].CurrentItemUIEntity.gameObject);

                    _slots[i].CurrentItemUIEntity = null;
                    _slots[i - 1].CurrentItemUIEntity = null;
                    _slots[i - 2].CurrentItemUIEntity = null;

                    ShiftLeftForMatch(i);
                    // After shifting, check again from the start
                    //CheckTripleMerge();
                    return;
                }
            }
        }

        private ItemMatchSlot GetProperSlot(string itemId)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].CurrentItemUIEntity == null)
                {
                    return _slots[i];
                }

                if (i > 0 && _slots[i - 1].ItemID == itemId && _slots[i].ItemID != itemId)
                {
                    ShiftRight(i);
                    return _slots[i];
                }
            }

            Debug.LogError("Cannot find slot for item: " + itemId + "");
            return null;
        }

        private void ShiftRight(int fromIndex)
        {
            for (int i = _slots.Length - 2; i >= fromIndex; i--)
            {
                _slots[i + 1].CurrentItemUIEntity = _slots[i].CurrentItemUIEntity;
            }

            MoveAllItemsToIndexPositions();
        }

        private void ShiftLeftForMatch(int fromIndex)
        {
            int startIndex = fromIndex - 2;
            int lastIndex = Mathf.Min(fromIndex, _slots.Length - 4);
            for (int i = startIndex; i <= lastIndex; i++)
            {
                _slots[i].CurrentItemUIEntity = _slots[i + 3].CurrentItemUIEntity;
            }

            MoveAllItemsToIndexPositions();
        }

        private void MoveAllItemsToIndexPositions()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].CurrentItemUIEntity == null) break;
                var relativePosition = _slots[i].RelativePosition;
                _slots[i].CurrentItemUIEntity.MoveToSlotFromWorld(relativePosition, _slotSize);
            }
        }

        private bool HasEmptySlot() => _slots.Any(slot => slot.CurrentItemUIEntity == null);

        private Vector3 GetRelativeLocalPosition(Transform slotTransform)
        {
            return _itemUIEntityParent.InverseTransformPoint(slotTransform.position);
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
    }
}