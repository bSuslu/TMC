using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMC._Project.Gameplay.CityMatch.Scripts.GoalItem
{
    public class GoalItemView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;
        private GoalItemModel _goalItemModel;

        public void Initialize(GoalItemModel goalItemModel)
        {
            _goalItemModel = goalItemModel;
            goalItemModel.OnAmountChanged += UpdateAmount;

            SetIcon(goalItemModel.Icon);
            _icon.preserveAspect = true;
            SetAmount(goalItemModel.Amount);
        }

        private void OnDestroy()
        {
            _goalItemModel.OnAmountChanged -= UpdateAmount;
        }

        private void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }

        private void SetAmount(int amount)
        {
            _amountText.text = $"X {amount}";
        }
        
        private void UpdateAmount()
        {
            SetAmount(_goalItemModel.Amount);
        }
    }
}