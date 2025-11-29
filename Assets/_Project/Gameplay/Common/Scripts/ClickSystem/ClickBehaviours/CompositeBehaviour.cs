using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using TMC._Project.Gameplay.Common.Scripts.ClickSystem.ClickBehaviours.Base;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.ClickSystem.ClickBehaviours
{
    [CreateAssetMenu(menuName = "SO/Click Behaviours/Composite Behaviour")]
    public class CompositeBehaviour : ClickBehaviour
    {
        [SerializeField] private ClickBehaviour[] _behaviours;
    
        public override void Execute(ItemEntity clickedObject)
        {
            foreach (var behaviour in _behaviours)
            {
                behaviour.Execute(clickedObject);
            }
        }
    }
}