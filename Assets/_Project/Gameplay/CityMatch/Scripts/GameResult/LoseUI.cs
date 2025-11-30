using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;
using TMC._Project.Gameplay.Common.Scripts.UI;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.GameResult
{
    public class LoseUI: ResultUI
    {
        [SerializeField] private Transform _costContainer;
        
        private void Start()
        {
            LevelService levelService = ServiceLocator.Global.Get<LevelService>();
            var costs = levelService.ActiveLevelConfig.Costs;

            foreach (var cost in costs)
            {
                if (cost.ShowOutcomeWidget)
                {
                    BaseWidget widget = Instantiate(cost.OutcomeWidgetPrefab, _costContainer);
                    cost.SetWidget(widget);
                }
            }
        }
    }
}