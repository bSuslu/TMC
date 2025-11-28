using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    [CreateAssetMenu(fileName = "ItemSettings", menuName = "SO/Item/Settings")]
    public class ItemSettings: ScriptableObject
    {
        [SerializeField] private List<ItemConfig> _itemConfigs;
        public Dictionary<string, ItemConfig> ItemConfigs { get; private set; }
        
        public void Initialize()
        {
            ItemConfigs = _itemConfigs.ToDictionary(x => x.Id, x => x);
        }
    }
}