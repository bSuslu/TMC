using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.EventBus.Implementations;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.TimeSystem.Events;
using _Project.Core.Systems.TimeSystem.Interfaces;
using Cysharp.Threading.Tasks;
using TMC._Project.Gameplay.CityMatch.Scripts.Events;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;
using TMC._Project.Gameplay.Common.Scripts.OutcomeSystem;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.GameResult
{
    public class GameResultManager : MonoBehaviour
    {
        [SerializeField] private WinUI _winPanel;
        [SerializeField] private LoseUI _losePanel;

        private EventBinding<AllGoalItemsCollectedEvent> _allGoalItemsCollectedBinding;
        private EventBinding<TimeExpiredEvent> _timeExpiredBinding;
        private EventBinding<SlotsFullEvent> _slotsFullBinding;

        private LevelService _levelService;

        private void Start()
        {
            _levelService = ServiceLocator.Global.Get<LevelService>();

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

        private void OnSlotsFull() => Lose();

        private void OnAllGoalItemsCollected() => Win();


        private void OnTimerExpired() => Lose();


        private void Win()
        {
            StopTimer();
            var rewards = _levelService.ActiveLevelConfig.Rewards;
            GiveRewards(rewards);
            _winPanel.SetOutcomes(rewards);
            _winPanel.gameObject.SetActive(true);
            
            _levelService.CompleteLevel().Forget();
        }

        private void Lose()
        {
            StopTimer();
            var penalties = _levelService.ActiveLevelConfig.Penalties;
            _losePanel.gameObject.SetActive(true);
            GivePenalties(penalties);
            _losePanel.SetOutcomes(penalties);
        }

        private void StopTimer()
        {
            ServiceLocator.ForSceneOf(this).Get<IGameTimerService>().StopTimer();
        }

        private void GiveRewards(Outcome[] rewards)
        {
            foreach (var reward in rewards)
                reward.ApplyOutcome();
        }

        private void GivePenalties(Outcome[] penalties)
        {
            foreach (var penalty in penalties)
                penalty.ApplyOutcome();
        }
    }
}