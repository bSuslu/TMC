using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    public class LevelLoadManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _backgroundParent;
        [SerializeField] private Transform _itemParent;
        
        private LevelConfig _levelConfig;
        private ItemSettings _itemSettings;
        
        private void Start()
        {
            _levelConfig = ServiceLocator.Global.Get<LevelService>().ActiveLevelConfig;
            _itemSettings = ServiceLocator.Global.Get<ItemSettings>();

            LoadLevel();
        }

        private void LoadLevel()
        {
            SetBackgroundFromLevelConfig();
            SetItemsFromLevelConfig();
            SetCameraFromConfig();
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
                var itemConfig = _itemSettings.GetItemConfig(placementData.ItemId);
                var itemInstance = Instantiate(itemConfig.GetItemEntity(placementData.FaceDirection), _itemParent);
                itemInstance.transform.localPosition = placementData.Position;
            }
        }

        private void SetBackgroundFromLevelConfig()
        {
            foreach (var backgroundObject in _levelConfig.Backgrounds)
            {
                var background = Instantiate(backgroundObject, _backgroundParent);
                background.transform.localPosition = Vector3.zero;
            }
        }
    }
}
