using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts
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