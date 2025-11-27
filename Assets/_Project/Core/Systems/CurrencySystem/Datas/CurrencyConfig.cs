using UnityEngine;

namespace _Project.Core.Systems.CurrencySystem.Datas
{
    [CreateAssetMenu(fileName = "CurrencyConfig", menuName = "SO/Currency/Config")]
    public class CurrencyConfig : ScriptableObject
    {
        [field: SerializeField] public CurrencyType Type { get; private set; }
        [field: SerializeField] public bool UnlockedByDefault { get; private set; }
        [field: SerializeField] public int InitialAmount { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}