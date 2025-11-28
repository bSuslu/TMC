using TMC._Project.Core.Common.Utilities;
using TMC._Project.Gameplay.Common.Scripts.Enums;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "SO/Item/Config")]
    public class ItemConfig : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }

        [field: SerializeField]
        public SerializableDictionary<IsometricFaceDirection, GameObject> Prefab { get; private set; }
    }
}