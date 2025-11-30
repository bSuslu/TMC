using TMC._Project.Gameplay.Common.Scripts.OutcomeSystem;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "SO/Level/Config")]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public int DurationInSeconds { get; private set; }
        [field: SerializeField] public Outcome[] Rewards { get; private set; }
        [field: SerializeField] public Outcome[] Penalties { get; private set; }
        [field: SerializeField] public GameObject[] Backgrounds { get; set; }
        [field: SerializeField] public LevelItemRequirement[] GoalItems { get; set; }
        [field: SerializeField] public LevelCameraData InitialCameraData { get; set; }
        [field: SerializeField] public LevelItemPlacementData[] ItemPlacements { get; set; }
        
    }
}