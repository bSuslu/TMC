using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "Scriptable Objects/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}