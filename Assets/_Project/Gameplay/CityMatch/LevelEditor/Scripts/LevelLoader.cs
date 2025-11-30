#if UNITY_EDITOR
using NaughtyAttributes;
using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.LevelEditor.Scripts
{
    // TODO: For backgrounds, need a get prefab system maybe with a SO and list of prefabs to choose from
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private ItemSettings _itemSettings;
        [SerializeField] private Transform _backgroundParent;
        [SerializeField] private Transform _itemParent;

        [SerializeField, OnValueChanged(nameof(ValidateItemPlacements))]
        private LevelConfig _levelConfig;

        [Button]
        private void LoadLevelFromConfig()
        {
            CleanOldBackground();
            CleanOldItems();

            SetBackgroundFromLevelConfig();
            SetItemsFromLevelConfig();
            SetCameraFromConfig();
        }

        [Button]
        private void CreateNewLevelConfig()
        {
            var levelConfig = ScriptableObject.CreateInstance<LevelConfig>();
            SaveCameraToLevelConfig(levelConfig);
            // SaveBackgroundsToLevelConfig(levelConfig);
            SaveItemsToLevelConfig(levelConfig);

            string folderPath = "Assets/_Project/Gameplay/CityMatch/SO/Level/Configs";

            string baseName = "NewLevelConfig";
            string assetPath = $"{folderPath}/{baseName}.asset";
            assetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(assetPath);

            UnityEditor.AssetDatabase.CreateAsset(levelConfig, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            _levelConfig = levelConfig;

            // ValidateItemPlacements();
        }

        [Button]
        private void SaveToExistingLevelConfig()
        {
            SaveCameraToLevelConfig(_levelConfig);
            // SaveBackgroundsToLevelConfig(_levelConfig);
            SaveItemsToLevelConfig(_levelConfig);
            ValidateItemPlacements();
        }

        private void CleanOldBackground()
        {
            foreach (Transform child in _backgroundParent)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        private void SetCameraFromConfig()
        {
            _camera.orthographicSize = _levelConfig.InitialCameraData.Size;
            _camera.transform.position = _levelConfig.InitialCameraData.Position;
        }

        private void SetItemsFromLevelConfig()
        {
            var placementsData = _levelConfig.ItemPlacements;
            foreach (var placementData in placementsData)
            {
                var itemConfig = _itemSettings.GetItemConfigEditor(placementData.ItemId);
                var itemInstance = Instantiate(itemConfig.GetItemEntity(placementData.FaceDirection), _itemParent);
                itemInstance.transform.localPosition = placementData.Position;
            }
        }

        private void SetBackgroundFromLevelConfig()
        {
            if (_levelConfig.Backgrounds==null || _levelConfig.Backgrounds.Length==0)
            {
                Debug.LogWarning("LevelLoader: No backgrounds found in level config.");
                return;
            }
            foreach (var backgroundObject in _levelConfig.Backgrounds)
            {
                var background = Instantiate(backgroundObject, _backgroundParent);
                background.transform.localPosition = Vector3.zero;
            }
        }

        private void CleanOldItems()
        {
            foreach (Transform child in _itemParent)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        private void SaveCameraToLevelConfig(LevelConfig levelConfig)
        {
            levelConfig.InitialCameraData = new LevelCameraData
            {
                Position = _camera.transform.position,
                Size = _camera.orthographicSize
            };
        }

        private void SaveBackgroundsToLevelConfig(LevelConfig levelConfig)
        {
            GameObject[] backgrounds = new GameObject[_backgroundParent.childCount];
            int index = 0;

            foreach (Transform child in _backgroundParent)
            {
                backgrounds[index] = child.gameObject;
                index++;
            }

            levelConfig.Backgrounds = backgrounds;
        }

        private void SaveItemsToLevelConfig(LevelConfig levelConfig)
        {
            var entities = _itemParent.GetComponentsInChildren<ItemEntity>();

            levelConfig.ItemPlacements = new LevelItemPlacementData[entities.Length];

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var placementData = new LevelItemPlacementData
                {
                    ItemId = entity.Id,
                    Position = entity.transform.localPosition,
                    FaceDirection = entity.FaceDirection
                };
                levelConfig.ItemPlacements[i] = placementData;
            }
        }

        private void ValidateItemPlacements()
        {
            if (_levelConfig == null) return;

            var placements = _levelConfig.ItemPlacements;
            var goalItems = _levelConfig.GoalItems;

            foreach (var goal in goalItems)
            {
                int countInPlacements = 0;

                foreach (var placement in placements)
                {
                    if (placement.ItemId == goal.Config.Id)
                        countInPlacements++;
                }

                if (countInPlacements < goal.Amount)
                {
                    Debug.LogWarning(
                        $"LevelLoader: Item '{goal.Config.Id}' should appear at least {goal.Amount} times in ItemPlacements, but found {countInPlacements}.");
                }
            }
        }
    }
}

#endif