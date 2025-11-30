using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.EventBus.Implementations;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.TimeSystem.Events;
using _Project.Core.Systems.TimeSystem.Interfaces;
using TMC._Project.Gameplay.CityMatch.Scripts.Events;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.GameResult
{
    public class GameResultManager : MonoBehaviour
    {
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;
        
        private EventBinding<AllGoalItemsCollectedEvent> _allGoalItemsCollectedBinding;
        private EventBinding<TimeExpiredEvent> _timeExpiredBinding;
        private EventBinding<SlotsFullEvent> _slotsFullBinding;
        
        private void Start()
        {
            _allGoalItemsCollectedBinding = new EventBinding<AllGoalItemsCollectedEvent>(OnAllGoalItemsCollected);
            EventBus<AllGoalItemsCollectedEvent>.Subscribe(_allGoalItemsCollectedBinding);
            
            _timeExpiredBinding = new EventBinding<TimeExpiredEvent>(OnTimerExpired);
            EventBus<TimeExpiredEvent>.Subscribe(_timeExpiredBinding);
            
            _slotsFullBinding = new EventBinding<SlotsFullEvent>(OnSlotsFull);
            EventBus<SlotsFullEvent>.Subscribe(_slotsFullBinding);
        }

        private void OnDestroy()
        {
            EventBus<AllGoalItemsCollectedEvent>.Unsubscribe(_allGoalItemsCollectedBinding);
            EventBus<TimeExpiredEvent>.Unsubscribe(_timeExpiredBinding);
            EventBus<SlotsFullEvent>.Unsubscribe(_slotsFullBinding);
        }

        private void OnSlotsFull()
        {
            StopTimer();
            _losePanel.SetActive(true);
        }
        private void OnAllGoalItemsCollected()
        {
            StopTimer();
            _winPanel.SetActive(true);
        }
        
        private void OnTimerExpired()
        {
            StopTimer();
            _losePanel.SetActive(true);
        }
        
        private void StopTimer()
        {
            ServiceLocator.ForSceneOf(this).Get<IGameTimerService>().StopTimer();
        }
    }
}
