using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.ClickSystem.ClickBehaviours
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