using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemMatchSlot
    {
        public ItemUIEntity CurrentItemUIEntity { get; set; }
        public string ItemID => CurrentItemUIEntity!=null? CurrentItemUIEntity.ItemId: "-1"; 
        public readonly Vector2 RelativePosition;
        
        public ItemMatchSlot (Vector2 relativePosition)
        {
            RelativePosition = relativePosition;
        }
    }
    
   
}