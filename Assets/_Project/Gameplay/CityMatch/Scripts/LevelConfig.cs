using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public int DurationInSeconds { get; private set; }
        [field: SerializeField] public LevelCameraData InitialCameraData { get; private set; }
        [field: SerializeField] public LevelItemRequirement[] ItemRequirements { get; private set; }
        [field: SerializeField] public LevelItemPlacementData[] ItemPlacements { get; private set; }
    }
}