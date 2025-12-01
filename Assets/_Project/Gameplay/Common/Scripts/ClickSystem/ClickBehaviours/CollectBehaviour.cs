using _Project.Core.Framework.EventBus;
using TMC._Project.Gameplay.CityMatch.Scripts.Events;
using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using TMC._Project.Gameplay.Common.Scripts.ClickSystem.ClickBehaviours.Base;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.ClickSystem.ClickBehaviours
{
    [CreateAssetMenu(menuName = "SO/Click Behaviours/Collect Behaviour")]
    public class CollectBehaviour : ClickBehaviour
    {
        public override void Execute(ItemEntity itemEntity)
        {
            EventBus<CollectableItemClickedEvent>.Publish(new CollectableItemClickedEvent(itemEntity.Id));
        }
    }
}