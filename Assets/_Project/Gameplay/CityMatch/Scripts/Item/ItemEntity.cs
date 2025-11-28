using TMC._Project.Gameplay.Common.ClickSystem;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemEntity : MonoBehaviour, IClickable
    {
        public ItemConfig Config;

        public void HandleClick()
        {
            Debug.Log($"Clicked on Item: {Config.Name}");
        }
    }
}