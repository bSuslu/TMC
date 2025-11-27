using Cysharp.Threading.Tasks;

namespace _Project.Core.Systems.TimeSystem.Interfaces
{
    public interface IGameTimerService
    {
        UniTask StartTimerAsync(float duration);
        void PauseTimer();
        void ResumeTimer();
        void StopTimer();
        void AddTime(float secondsToAdd);
        float GetRemainingTime();
        string GetFormattedTime();

        event System.Action<string> OnTimeFormatted;
        event System.Action OnTimerStarted;
        event System.Action OnTimerExpired;
        event System.Action OnTimerPaused;
        event System.Action OnTimerResumed;
    }
}