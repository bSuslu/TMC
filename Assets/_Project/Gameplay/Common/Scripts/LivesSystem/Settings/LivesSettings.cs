using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.LivesSystem.Settings
{
    [CreateAssetMenu(fileName = "LivesSettings", menuName = "SO/Lives/Settings")]
    public class LivesSettings : ScriptableObject
    {
        [field: SerializeField] public int StartLives { get; private set; }
        [field: SerializeField] public int MaxLives { get; private set; }
        
        [field: SerializeField] public float RegenTime { get; private set; }
    }
}