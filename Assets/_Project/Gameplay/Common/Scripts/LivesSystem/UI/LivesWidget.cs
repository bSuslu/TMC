using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Service;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMC._Project.Gameplay.Common.Scripts.LivesSystem.UI
{
    public class LivesWidget : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;

        private LivesService _livesService;
        private LivesSettings _livesSettings;

        private void Awake()
        {
            _livesService = ServiceLocator.Global.Get<LivesService>();
            _livesSettings = ServiceLocator.Global.Get<LivesSettings>();

            
            _amountText.text = _livesService.Data.CurrentLives.ToString();
        }

        private void UpdateText()
        {
            _amountText.text = _livesService.Data.CurrentLives.ToString();
        }
    }
}