using System;

namespace _Project.Core.Systems.TimeSystem.Data
{
    [Serializable]
    public class TimerData
    {
        public float Duration;
        public float TimeRemaining;
        public bool IsRunning;
        public bool IsPaused;
        public Action OnTimerExpired { get; set; }
        public Action<float> OnTimerTick { get; set; }

        public TimerData(float duration)
        {
            Duration = duration;
            TimeRemaining = duration;
            IsRunning = false;
            IsPaused = false;
        }
    }
}