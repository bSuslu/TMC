using _Project.Core.Framework.EventBus.Interfaces;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Events
{
    public struct CollectableItemCollectedEvent: IEvent
    {
        public string ItemId;
        
        public CollectableItemCollectedEvent(string itemId)
        {
            ItemId = itemId;
        }
    }
}