using _Project.Core.Systems.TimeSystem.Data;
using _Project.Core.Systems.TimeSystem.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using _Project.Core.Framework.EventBus;
using _Project.Core.Systems.TimeSystem.Events;

namespace _Project.Core.Systems.TimeSystem.Services
{
    public class GameTimerService : IGameTimerService
    {
        private TimerData _currentTimer;
        private bool _isTimerActive = false;
        private float _lastKnownTime;
        public event Action<string> OnTimeFormatted;
        public event Action OnTimerStarted;
        public event Action OnTimerExpired;
        public event Action OnTimerPaused;
        public event Action OnTimerResumed;

        public async UniTask StartTimerAsync(float duration)
        {
            if (_isTimerActive)
            {
                Debug.LogWarning("Timer is already running!");
                return;
            }

            _currentTimer = new TimerData(duration);
            _isTimerActive = true;

            OnTimerStarted?.Invoke();

            // Timer loop
            while (_isTimerActive && _currentTimer.TimeRemaining > 0)
            {
                if (!_currentTimer.IsPaused)
                {
                    _currentTimer.TimeRemaining -= Time.deltaTime;

                    // Update UI every frame
                    string formattedTime = TimeFormatter.FormatTimeMMSS(_currentTimer.TimeRemaining);
                    OnTimeFormatted?.Invoke(formattedTime);

                    _currentTimer.OnTimerTick?.Invoke(_currentTimer.TimeRemaining);
                }

                await UniTask.Yield();
            }

            if (_isTimerActive && _currentTimer.TimeRemaining <= 0)
            {
                OnTimerExpired?.Invoke();
                EventBus<TimeExpiredEvent>.Publish(new TimeExpiredEvent());
                _currentTimer.OnTimerExpired?.Invoke();
            }

            _isTimerActive = false;
        }

        public void PauseTimer()
        {
            if (_currentTimer != null && !_currentTimer.IsPaused)
            {
                _currentTimer.IsPaused = true;
                OnTimerPaused?.Invoke();
            }
        }

        public void ResumeTimer()
        {
            if (_currentTimer != null && _currentTimer.IsPaused)
            {
                _currentTimer.IsPaused = false;
                OnTimerResumed?.Invoke();
            }
        }

        public void StopTimer()
        {
            _lastKnownTime = _currentTimer.TimeRemaining;
            _isTimerActive = false;
            _currentTimer = null;
        }

        public void AddTime(float secondsToAdd)
        {
            if (_currentTimer != null)
            {
                _currentTimer.TimeRemaining += secondsToAdd;
            }
        }

        public float GetRemainingTime() => _currentTimer?.TimeRemaining ?? 0f;

        public string GetFormattedTime()
            => TimeFormatter.FormatTimeMMSS(_currentTimer?.TimeRemaining ?? 0f);
        
        public float GetLastTime() => _lastKnownTime;
        public string GetLastFormattedTime()
        => TimeFormatter.FormatTimeMMSS(_lastKnownTime);
        
    }
}