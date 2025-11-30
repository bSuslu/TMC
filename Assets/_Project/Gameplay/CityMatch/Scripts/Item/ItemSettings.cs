using System.Collections.Generic;
using System.Linq;
using TMC._Project.Core.Common.Bases;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    [CreateAssetMenu(fileName = "ItemSettings", menuName = "SO/Item/Settings")]
    public class ItemSettings: BaseSettings
    {
        [SerializeField] private List<ItemConfig> _itemConfigs;
        public Dictionary<string, ItemConfig> ItemConfigs { get; private set; }
        
        public override void Initialize()
        {
            ItemConfigs = _itemConfigs.ToDictionary(x => x.Id, x => x);
        }
        
        public ItemConfig GetItemConfig(string id)
        {
            return ItemConfigs[id];
        }
    }
}