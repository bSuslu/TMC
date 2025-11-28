using _Project.Core.Framework.EventBus.Interfaces;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Events
{
    public struct CollectableItemClickedEvent : IEvent
    {
        public string ItemId;
        
        public CollectableItemClickedEvent(string itemId)
        {
            ItemId = itemId;
        }
        
    }
}