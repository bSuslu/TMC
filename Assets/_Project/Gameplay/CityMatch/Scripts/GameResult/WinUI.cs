using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.TimeSystem.Interfaces;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;
using TMC._Project.Gameplay.Common.Scripts.UI;
using TMPro;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.GameResult
{
    public class WinUI : ResultUI
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Transform _rewardContainer;
        
        private void Start()
        {
            _timerText.text = ServiceLocator.ForSceneOf(this).Get<IGameTimerService>().GetLastFormattedTime();
            
            LevelService levelService = ServiceLocator.Global.Get<LevelService>();
            var rewards = levelService.ActiveLevelConfig.Rewards;

            foreach (var reward in rewards)
            {
                if (reward.ShowOutcomeWidget)
                {
                    BaseWidget widget = Instantiate(reward.OutcomeWidgetPrefab, _rewardContainer);
                    reward.SetWidget(widget);
                }
            }
        }
    }
}