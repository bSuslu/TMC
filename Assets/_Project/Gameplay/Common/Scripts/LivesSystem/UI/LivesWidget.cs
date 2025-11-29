using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.Common.Scripts.LivesSystem.Service;
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

        private void Awake()
        {
            _livesService = ServiceLocator.Global.Get<LivesService>();
            _amountText.text = _livesService.Data.CurrentLives.ToString();

            _livesService.OnLivesChanged += OnLivesChanged;
        }

        private void OnDestroy()
        {
            _livesService.OnLivesChanged -= OnLivesChanged;
        }

        private void OnLivesChanged(int obj)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            _amountText.text = _livesService.Data.CurrentLives.ToString();
        }
    }
}