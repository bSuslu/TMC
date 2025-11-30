using NaughtyAttributes;
using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Editor
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField]private Camera _camera;
        [SerializeField]private ItemSettings _itemSettings;
        
        [SerializeField, OnValueChanged(nameof(LoadNewLevel))] private LevelConfig _levelConfig;

        [SerializeField] private Transform _levelTransform;
        
        [Button]
        private void LoadNewLevel()
        {
            if(_levelTransform != null) DestroyImmediate(_levelTransform.gameObject);
            if(_levelConfig != null)
            {
                _levelTransform = new GameObject(_levelConfig.name).transform;
                _levelTransform.SetParent(transform);
            }

            _camera.orthographicSize = _levelConfig.InitialCameraData.Size;
            _camera.transform.position = _levelConfig.InitialCameraData.Position;
            
            var backGround = new GameObject("Background");
            backGround.transform.SetParent(_levelTransform);
            backGround.transform.localPosition = Vector3.zero;
            backGround.AddComponent<SpriteRenderer>().sprite = _levelConfig.BackgroundImage;
            backGround.transform.localScale = new Vector3(100, 100, 1);
            
            var entityParent = new GameObject("Items");
            entityParent.transform.SetParent(_levelTransform);

            var placementsData = _levelConfig.ItemPlacements;
            foreach (var placementData in placementsData)
            {
                var itemConfig = _itemSettings.GetItemConfig(placementData.ItemId);
                var itemInstance = Instantiate(itemConfig.GetItemEntity(placementData.FaceDirection), entityParent.transform);
                itemInstance.transform.localPosition = placementData.Position;
            }
        }
        
        [Button]
        private void SaveLevelToConfig()
        {
            var entities = _levelTransform.GetComponentsInChildren<ItemEntity>();

            _levelConfig.ItemPlacements = new LevelItemPlacementData[entities.Length];

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var placementData = new LevelItemPlacementData
                {
                    ItemId = entity.Id,
                    Position = entity.transform.localPosition,
                    FaceDirection = entity.FaceDirection
                };
                _levelConfig.ItemPlacements[i] = placementData;
            }
        }
    }
}
