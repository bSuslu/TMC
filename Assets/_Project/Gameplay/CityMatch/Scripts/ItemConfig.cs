using TMC._Project.Core.Common.Utilities;
using TMC._Project.Gameplay.Common;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "Scriptable Objects/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public SerializableDictionary<IsometricFaceDirection, GameObject> Prefab { get; private set; }
    }
}