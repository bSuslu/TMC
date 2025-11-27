using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemMatchSlotView : MonoBehaviour
    {
        public int Index { get; set; }

        private void Start()
        {
            Index = transform.GetSiblingIndex();
        }
    }
}