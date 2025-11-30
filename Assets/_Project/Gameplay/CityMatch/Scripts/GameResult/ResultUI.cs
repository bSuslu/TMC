using TMC._Project.Gameplay.Common.Scripts.OutcomeSystem;
using TMC._Project.Gameplay.Common.Scripts.UI;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.GameResult
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] private Transform _outcomeContainer;
        
        private Outcome[] _outcomes;
        
        public void SetOutcomes(Outcome[] outcomes) => _outcomes = outcomes;

        protected void Show()
        {
            foreach (var outcome in _outcomes)
            {
                if (outcome.ShowOutcomeWidget)
                {
                    BaseWidget widget = Instantiate(outcome.OutcomeWidgetPrefab, _outcomeContainer);
                    outcome.SetWidget(widget);
                }
            }
        }
    }
}