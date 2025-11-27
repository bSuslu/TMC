using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    public class ItemEntity : MonoBehaviour,IClickable
    {
        public int ItemId;
        public void OnClick()
        {
            Debug.Log($"Item {ItemId} clicked");
        }
    }
}