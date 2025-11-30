using System;

namespace _Project.Core.Systems.TimeSystem.Data
{
    [Serializable]
    public class TimerData
    {
        public float Duration;
        public float TimeRemaining;
        public bool IsPaused;

        public TimerData(float duration)
        {
            Duration = duration;
            TimeRemaining = duration;
            IsPaused = false;
        }
    }
}