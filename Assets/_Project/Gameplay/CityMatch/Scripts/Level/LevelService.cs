using System.Collections.Generic;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.LoadingSystem.Interfaces;
using _Project.Core.Systems.SaveSystem.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Level
{
    public class LevelService: IAsyncService
    {
        private readonly LevelSettings _levelSettings;
        private readonly ISaveService _saveService;

        private const string k_saveKey = "citymatch_levels";
        private Dictionary<int, LevelData> _levelData = new();
        public LevelConfig CurrentLevelConfig { get; private set; }
        public int CurrentLevelId { get; private set; } = 1;

        public LevelService()
        {
            _levelSettings = ServiceLocator.Global.Get<LevelSettings>();
            _saveService = ServiceLocator.Global.Get<ISaveService>();
        }

        public async UniTask InitializeAsync()
        {
            Debug.Log("Initializing LevelService");
            await LoadLevelData();
            Debug.Log("LevelService Initialized");
        }

        private async UniTask LoadLevelData()
        {
            var (success, savedData) = await _saveService.TryLoadAsync<Dictionary<int, LevelData>>(k_saveKey);

            if (success && savedData != null)
            {
                _levelData = savedData;
            }
            else
            {
                CreateDefaultLevelData();
            }
        }

        private void CreateDefaultLevelData()
        {
            _levelData.Clear();

            for (int i = 1; i <= _levelSettings.LevelCount; i++)
            {
                _levelData[i] = new LevelData(i);
            }
        }

        public bool StartLevel(int levelId)
        {
            var levelConfig = _levelSettings.GetLevelConfig(levelId);
            if (levelConfig == null)
            {
                Debug.LogError($"Level {levelId} config not found!");
                return false;
            }

            var levelData = GetLevelData(levelId);
            if (!levelData.IsUnlocked)
            {
                Debug.LogWarning($"Level {levelId} is locked!");
                return false;
            }

            CurrentLevelId = levelId;
            CurrentLevelConfig = levelConfig;

            Debug.Log($"Level {levelId} started!");
            return true;
        }

        public async UniTask CompleteLevel(int score, int stars)
        {
            var levelData = GetLevelData(CurrentLevelId);

            // Update level data
            levelData.IsCompleted = true;
            levelData.StarsEarned = Mathf.Max(levelData.StarsEarned, stars);

            // Sonraki level'i unlock et
            UnlockNextLevel();

            // Save et
            await SaveLevelData();

            Debug.Log($"Level {CurrentLevelId} completed! Score: {score}, Stars: {stars}");
        }

        public LevelData GetLevelData(int levelId)
        {
            if (!_levelData.ContainsKey(levelId))
            {
                _levelData[levelId] = new LevelData(levelId);
            }

            return _levelData[levelId];
        }

        public bool HasNextLevel() => CurrentLevelId < _levelSettings.LevelCount;

        public bool IsLevelUnlocked(int levelId) => GetLevelData(levelId).IsUnlocked;

        private void UnlockNextLevel()
        {
            int nextLevelId = CurrentLevelId + 1;
            if (nextLevelId <= _levelSettings.LevelCount)
            {
                var nextLevelData = GetLevelData(nextLevelId);
                nextLevelData.IsUnlocked = true;
            }
        }

        private async UniTask SaveLevelData()
        {
            await _saveService.SaveAsync(k_saveKey, _levelData);
        }
    }
}