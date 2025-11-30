using TMC._Project.Gameplay.Common.Scripts.UI;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.OutcomeSystem
{
    public abstract class Outcome : ScriptableObject
    {
        public bool ShowOutcomeWidget;
        public BaseWidget OutcomeWidgetPrefab;
        public abstract void ApplyOutcome();

        public virtual void SetWidget(BaseWidget widget)
        {
            
        }
    }
}