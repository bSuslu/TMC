using System.Collections.Generic;
using _Project.Core.Common.Bases;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "SO/Level/Setting")]
    public class LevelSettings : BaseSettings
    {
        [field: SerializeField] public List<LevelConfig> LevelConfigs { get; private set; }
        
        public int LevelCount => LevelConfigs.Count;
        public LevelConfig GetLevelConfig(int levelId)
        {
            if (levelId < 1 || levelId > LevelConfigs.Count)
                return null;
                
            return LevelConfigs[levelId - 1]; // Level 1 = index 0
        }
    }
}