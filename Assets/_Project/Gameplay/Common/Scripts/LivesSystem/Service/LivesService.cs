using System;
using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.LoadingSystem.Interfaces;
using _Project.Core.Systems.SaveSystem.Interfaces;
using Cysharp.Threading.Tasks;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Data;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Settings;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.LivesSystem.Service
{
    public class LivesService : IAsyncService
    {
        public LivesData Data { get; private set; }

        public event Action<int> OnLivesChanged;
        public event Action OnLivesDepleted;
        public event Action OnLivesRegenStarted;
        public event Action OnLivesRegenCompleted;
        public event Action OnLivesFull;

        private readonly LivesSettings _livesSettings;
        private readonly ISaveService _saveService;
        private const string k_saveKey = "lives_data";

        public LivesService()
        {
            _livesSettings = ServiceLocator.Global.Get<LivesSettings>();
            _saveService = ServiceLocator.Global.Get<ISaveService>();
        }

        public async UniTask InitializeAsync()
        {
            var (success, loadedData) = await _saveService.TryLoadAsync<LivesData>(k_saveKey);

            if (success && loadedData != null)
            {
                Data = loadedData;
                NormalizeData();
            }
            else
            {
                Data = new LivesData
                {
                    CurrentLives = _livesSettings.StartLives,
                    MaxLives = _livesSettings.MaxLives,
                    RegenTime = _livesSettings.RegenTime
                };
                await SaveAsync();
            }
        }

        private void NormalizeData()
        {
            bool changed = false;

            if (Data.CurrentLives > _livesSettings.MaxLives)
            {
                Data.CurrentLives = _livesSettings.MaxLives;
                changed = true;
            }

            if (Data.MaxLives != _livesSettings.MaxLives)
            {
                Data.MaxLives = _livesSettings.MaxLives;
                changed = true;
            }

            if (changed)
            {
                SaveAsync().Forget();
                OnLivesChanged?.Invoke(Data.CurrentLives);
            }
        }

        private async UniTask SaveAsync()
        {
            await _saveService.SaveAsync(k_saveKey, Data);
        }

        public void AddLives(int amount)
        {
            int oldLives = Data.CurrentLives;
            Data.CurrentLives = Mathf.Min(Data.CurrentLives + amount, _livesSettings.MaxLives);
            SaveAsync().Forget();

            if (Data.CurrentLives != oldLives)
            {
                OnLivesChanged?.Invoke(Data.CurrentLives);

                if (Data.CurrentLives == _livesSettings.MaxLives)
                    OnLivesFull?.Invoke();
            }
            Debug.Log($"[LivesService] trying to add {amount} lives. old lives: {oldLives}, new lives: {Data.CurrentLives}");
        }

        public void RemoveLives(int amount)
        {
            int oldLives = Data.CurrentLives;
            Data.CurrentLives = Mathf.Max(Data.CurrentLives - amount, 0);
            SaveAsync().Forget();

            if (Data.CurrentLives != oldLives)
            {
                OnLivesChanged?.Invoke(Data.CurrentLives);

                if (Data.CurrentLives == 0)
                    OnLivesDepleted?.Invoke();
            }
            Debug.Log($"[LivesService] trying to remove {amount} lives. old lives: {oldLives}, new lives: {Data.CurrentLives}");
        }

        public void SetCurrentLives(int amount)
        {
            int oldLives = Data.CurrentLives;
            Data.CurrentLives = Mathf.Clamp(amount, 0, _livesSettings.MaxLives);
            SaveAsync().Forget();

            if (Data.CurrentLives != oldLives)
            {
                OnLivesChanged?.Invoke(Data.CurrentLives);

                if (Data.CurrentLives == 0)
                    OnLivesDepleted?.Invoke();
                else if (Data.CurrentLives == _livesSettings.MaxLives)
                    OnLivesFull?.Invoke();
            }
        }

        public void StartRegen()
        {
            OnLivesRegenStarted?.Invoke();
        }

        public void CompleteRegen()
        {
            OnLivesRegenCompleted?.Invoke();
        }
    }
}