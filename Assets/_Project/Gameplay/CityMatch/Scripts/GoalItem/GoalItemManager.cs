using System.Collections.Generic;
using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.EventBus.Implementations;
using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.CityMatch.Scripts.Events;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.GoalItem
{
    public class GoalItemManager : MonoBehaviour
    {
        [SerializeField] private GoalItemView _goalItemViewPrefab;
        [SerializeField] private Transform _parent;

        private readonly Dictionary<string, GoalItemModel> _goalItemModels = new();
        private readonly Dictionary<string, GoalItemView> _goalItemViews = new();
        private LevelConfig _levelConfig;
        
        private EventBinding<CollectableItemCollectedEvent> _collectableItemCollectedEventBinding;
        
        private void Awake()
        {
            _levelConfig = ServiceLocator.Global.Get<LevelService>().ActiveLevelConfig;
            
            _collectableItemCollectedEventBinding = new EventBinding<CollectableItemCollectedEvent>(OnCollectableItemCollected);
            EventBus<CollectableItemCollectedEvent>.Subscribe(_collectableItemCollectedEventBinding);
        }
        private void OnDestroy()
        {
            EventBus<CollectableItemCollectedEvent>.Unsubscribe(_collectableItemCollectedEventBinding);
            EventBus<AllGoalItemsCollectedEvent>.Clear();
        }
        
        private void Start()
        {
            foreach (var goalItemConfig in _levelConfig.GoalItems)
            {
                GoalItemModel goalItemModel = new(goalItemConfig.Config.Icon, goalItemConfig.Amount);
                GoalItemView goalItemView = Instantiate(_goalItemViewPrefab, _parent);
                goalItemView.Initialize(goalItemModel);
                _goalItemModels.Add(goalItemConfig.Config.Id, goalItemModel);
                _goalItemViews.Add(goalItemConfig.Config.Id, goalItemView);
            }
        }

        private void OnCollectableItemCollected(CollectableItemCollectedEvent collectableItemClickedEvent)
        {
            if (_goalItemModels.TryGetValue(collectableItemClickedEvent.ItemId, out var goalItemModel))
            {
                if (goalItemModel.Amount >= 0)
                {
                    goalItemModel.DecreaseAmount(1);

                    if (goalItemModel.Amount == 0)
                    {
                        
                        Destroy(_goalItemViews[collectableItemClickedEvent.ItemId].gameObject);
                        _goalItemModels.Remove(collectableItemClickedEvent.ItemId);
                        _goalItemViews.Remove(collectableItemClickedEvent.ItemId);
                        
                        CheckIfAllGoalItemsCollected();
                    }
                }
            }
        }

        private void CheckIfAllGoalItemsCollected()
        {
            if (_goalItemModels.Count == 0)
            {
                EventBus<AllGoalItemsCollectedEvent>.Publish(new AllGoalItemsCollectedEvent());
            }
        }
    }
}
