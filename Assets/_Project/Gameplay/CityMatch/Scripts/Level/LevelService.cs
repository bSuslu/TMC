using System.Collections.Generic;
using _Project.Core.Framework.LogSystems;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.LoadingSystem.Interfaces;
using _Project.Core.Systems.SaveSystem.Interfaces;
using Cysharp.Threading.Tasks;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    public class LevelService : IAsyncService, ILevelService
    {
        private readonly LevelSettings _levelSettings;
        private readonly ISaveService _saveService;

        private const string k_saveKey = "citymatch_levels";
        private Dictionary<int, LevelData> _levelData = new();
        public int ActiveLevelId { get; private set; }
        public LevelConfig ActiveLevelConfig { get; private set; }

        public LevelService()
        {
            _levelSettings = ServiceLocator.Global.Get<LevelSettings>();
            _saveService = ServiceLocator.Global.Get<ISaveService>();
        }

        public async UniTask InitializeAsync()
        {
            Log.Info("LevelService: Initializing...");
            await LoadLevelData();
            Log.Info("LevelService: Initialized.");
        }

        public async UniTask LoadLevelData()
        {
            var (success, savedData) = await _saveService.TryLoadAsync<Dictionary<int, LevelData>>(k_saveKey);

            if (success && savedData != null)
            {
                _levelData = savedData;
            }
            else
            {
                CreateDefaultLevelData();
                await SaveAsync();
            }

            ActiveLevelId = GetLowestInCompleteLevelId();
            ActiveLevelConfig = _levelSettings.GetLevelConfig(ActiveLevelId);
        }

        public int GetLowestInCompleteLevelId()
        {
            foreach (var levelConfig in _levelSettings.LevelConfigs)
            {
                var data = GetLevelData(levelConfig.Id);
                if (data.IsCompleted == false)
                    return data.Id;
            }

            // Hiçbir level unlock değilse, ilk config ID’sini döndür
            return _levelSettings.LevelConfigs[0].Id;
        }

        public void CreateDefaultLevelData()
        {
            _levelData.Clear();

            for (int i = 0; i < _levelSettings.LevelCount; i++)
            {
                int configId = _levelSettings.LevelConfigs[i].Id;
                _levelData[configId] = new LevelData(configId);
            }
        }

        public bool StartLevel(int levelId)
        {
            var levelConfig = _levelSettings.GetLevelConfig(levelId);
            if (levelConfig == null)
            {
                Log.Error($"LevelService: Level {levelId} config not found!");
                return false;
            }

            var levelData = GetLevelData(levelId);
            if (!levelData.IsUnlocked)
            {
                Log.Warning($"LevelService: Level {levelId} is locked!");
                return false;
            }

            ActiveLevelId = levelId;
            ActiveLevelConfig = levelConfig;

            Log.Info($"LevelService: Level {levelId} started.");
            return true;
        }

        public async UniTask CompleteLevel()
        {
            var levelData = GetLevelData(ActiveLevelId);

            // Update level data
            levelData.IsCompleted = true;
            Log.Info($"LevelService: Level {ActiveLevelId} completed.");
            
            // Sonraki level'i unlock et
            UnlockNextLevel();
            
            // Active level’i yeni açılan level olarak ayarla
            int nextLevelId = ActiveLevelId + 1;
            if (nextLevelId <= _levelSettings.LevelCount)
            {
                ActiveLevelId = nextLevelId;
                ActiveLevelConfig = _levelSettings.GetLevelConfig(nextLevelId);
            }

            // Save et
            await SaveAsync();
        }

        public LevelData GetLevelData(int levelId)
        {
            if (!_levelData.ContainsKey(levelId))
            {
                _levelData[levelId] = new LevelData(levelId);
            }

            return _levelData[levelId];
        }

        public bool HasNextLevel() => ActiveLevelId < _levelSettings.LevelCount;

        public bool IsLevelUnlocked(int levelId) => GetLevelData(levelId).IsUnlocked;

        public void UnlockNextLevel()
        {
            int nextLevelId = ActiveLevelId + 1;
            if (nextLevelId <= _levelSettings.LevelCount)
            {
                var nextLevelData = GetLevelData(nextLevelId);
                nextLevelData.IsUnlocked = true;
            }
        }

        public async UniTask SaveAsync()
        {
            await _saveService.SaveAsync(k_saveKey, _levelData);
        }
    }
}