using TMC._Project.Gameplay.Common.Scripts.OutcomeActionSystem;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "SO/Level/Config")]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public int DurationInSeconds { get; private set; }
        [field: SerializeField] public GameObject LevelPrefab { get; private set; }
        [field: SerializeField] public Sprite BackgroundImage { get; private set; }
        [field: SerializeField] public OutcomeAction[] Rewards { get; private set; }
        [field: SerializeField] public OutcomeAction[] Costs { get; private set; }
        [field: SerializeField] public LevelItemRequirement[] GoalItems { get; private set; }
        [field: SerializeField] public LevelCameraData InitialCameraData { get; set; }
        [field: SerializeField] public LevelItemPlacementData[] ItemPlacements { get; set; }
    }
}