using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.CityMatch.Scripts.Level;
using TMPro;
using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.UI
{
    public class CurrentLevelText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;

        private void Start()
        {
            LevelService levelService = ServiceLocator.Global.Get<LevelService>();
            _levelText.text = levelService.CurrentLevelId.ToString();
        }
    }
}
