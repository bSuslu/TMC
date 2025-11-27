using UnityEngine;

namespace _Project.Core.Systems.TimeSystem.Services
{
    public static class TimeFormatter
    {
        public static string FormatTimeMMSS(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        public static string FormatTimeMMSSFFF(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000) % 1000);
            return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
        }

        public static (int minutes, int seconds, int milliseconds) SplitTime(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000) % 1000);
            return (minutes, seconds, milliseconds);
        }
    }
}