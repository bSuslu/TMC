using System;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.GoalItem
{
    public class GoalItemModel
    {
        public Sprite Icon;
        public int Amount;
        
        public event Action OnAmountChanged;
        public GoalItemModel(Sprite icon, int amount)
        {
            Icon = icon;
            Amount = amount;
        }

        public void IncreaseAmount(int amount)
        {
            Amount += amount;
            OnAmountChanged?.Invoke();
        }
        
        public void DecreaseAmount(int amount)
        {
            Amount -= amount;
            OnAmountChanged?.Invoke();
        }
    }
}