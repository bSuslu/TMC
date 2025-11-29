using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.LivesSystem.Settings
{
    public class LifeSettings : ScriptableObject
    {
        [field: SerializeField] public int StartLife { get; set; }
        [field: SerializeField] public int MaxLife { get; set; }
    }
}