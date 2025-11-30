using _Project.Core.Framework.ServiceLocator;
using _Project.Core.Systems.TimeSystem.Interfaces;
using TMC._Project.Gameplay.Common.Scripts.OutcomeSystem;
using TMPro;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.GameResult
{
    public class WinUI : ResultUI
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        
        private Outcome[] _outcomes;
        private void Start()
        {
            _timerText.text = ServiceLocator.ForSceneOf(this).Get<IGameTimerService>().GetLastFormattedTime();
            Show();
        }
    }
}