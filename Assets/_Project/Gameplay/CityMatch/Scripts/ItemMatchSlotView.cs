using UnityEngine;
using UnityEngine.UI;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    public class ItemMatchSlotView : MonoBehaviour
    {
        [field: SerializeField] public Image ItemImage { get; private set; }
        public int Index { get; set; }

        private void Start()
        {
            Index = transform.GetSiblingIndex();
        }
    }
}