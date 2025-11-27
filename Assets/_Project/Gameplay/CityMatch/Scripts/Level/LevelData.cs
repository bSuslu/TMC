using System;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    [Serializable]
    public class LevelData
    {
        public int LevelId;
        public bool IsCompleted;
        public bool IsUnlocked;
        public int StarsEarned;
        
        public LevelData(int levelId)
        {
            LevelId = levelId;
            IsCompleted = false;
            IsUnlocked = levelId == 1; // Sadece ilk level açık
            StarsEarned = 0;
        }
    }
}