using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using TMC._Project.Gameplay.Common.Scripts.ClickSystem.ClickBehaviours.Base;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.ClickSystem.ClickBehaviours
{
    [CreateAssetMenu(menuName = "SO/Click Behaviours/Destroy Behaviour")]
    public class DestroyBehaviour : ClickBehaviour
    {
        [SerializeField] private float _destroyDelay = 0f;
    
        public override void Execute(ItemEntity clickedObject)
        {
            Destroy(clickedObject, _destroyDelay);
        }
    }
}