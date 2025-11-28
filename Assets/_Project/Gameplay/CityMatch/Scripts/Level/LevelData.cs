using System;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    [Serializable]
    public class LevelData
    {
        public int Id;
        public bool IsCompleted;
        public bool IsUnlocked;
        
        public LevelData(int levelId)
        {
            Id = levelId;
            IsCompleted = false;
            IsUnlocked = levelId == 1; // Sadece ilk level açı
        }
    }
}