using System;
using TMC._Project.Gameplay.CityMatch.Scripts.Item;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    [Serializable]
    public struct LevelItemRequirement
    {
        // TODO: Make this Item Id for remote Level Adjustments
        public ItemConfig Config;
        public int Amount;
    }
}