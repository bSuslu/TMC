using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.TimeSystem.Interfaces;
using TMPro;
using UnityEngine;

namespace _Project.Core.Systems.TimeSystem.UI
{
    public class TimerUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _warningColor = Color.yellow;
        [SerializeField] private Color _criticalColor = Color.red;
        [SerializeField] private float _warningThreshold = 30f; // 30 seconds
        [SerializeField] private float _criticalThreshold = 10f; // 10 seconds
        
        private IGameTimerService _timerService;

        private void Start()
        {
            _timerService = ServiceLocator.ForSceneOf(this).Get<IGameTimerService>();
            
            _timerService.OnTimeFormatted += OnTimeUpdated;
            _timerService.OnTimerStarted += OnTimerStarted;
            _timerService.OnTimerExpired += OnTimerExpired;
        }

        private void OnDestroy()
        {
            if (_timerService != null)
            {
                _timerService.OnTimeFormatted -= OnTimeUpdated;
                _timerService.OnTimerStarted -= OnTimerStarted;
                _timerService.OnTimerExpired -= OnTimerExpired;
            }
        }

        private void OnTimeUpdated(string formattedTime)
        {
            _timerText.text = formattedTime;
            
            // Color coding based on time remaining
            float remainingTime = _timerService.GetRemainingTime();
            UpdateTextColor(remainingTime);
        }

        private void UpdateTextColor(float remainingTime)
        {
            if (remainingTime <= _criticalThreshold)
            {
                _timerText.color = _criticalColor;
                // Optional: Add shake effect or pulse animation
            }
            else if (remainingTime <= _warningThreshold)
            {
                _timerText.color = _warningColor;
            }
            else
            {
                _timerText.color = _normalColor;
            }
        }

        private void OnTimerStarted()
        {
            _timerText.gameObject.SetActive(true);
        }

        private void OnTimerExpired()
        {
            _timerText.text = "00:00";
            _timerText.color = _criticalColor;
            
            // Optional: Flash effect or game over sequence
        }
    }
}