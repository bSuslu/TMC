using Cysharp.Threading.Tasks;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    public interface ILevelService
    {
        int ActiveLevelId { get; }
        LevelConfig ActiveLevelConfig { get; }
        UniTask LoadLevelData();
        int GetLowestUnlockedLevelId();
        void CreateDefaultLevelData();
        bool StartLevel(int levelId);
        UniTask CompleteLevel();
        LevelData GetLevelData(int levelId);
        bool HasNextLevel();
        bool IsLevelUnlocked(int levelId);
        void UnlockNextLevel();
        UniTask SaveAsync();
    }
}