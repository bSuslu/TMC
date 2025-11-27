
using TMC._Project.Gameplay.CityMatch.Scripts.Item;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts
{
    public class ClickHandler: MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private ItemMatchView _itemMatchView;
        private readonly RaycastHit2D[] _results = new RaycastHit2D[10];
        private readonly ContactFilter2D _filter = new ();

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                int hitCount = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, _filter, _results);
                if (hitCount <= 0) return;
                int index = 0;
                float minY = float.MaxValue;

                for (int i = 0; i < hitCount; i++)
                {
                    if (_results[i].collider.transform.position.y < minY)
                    {
                        index = i;
                        minY = _results[i].collider.transform.position.y;
                    }
                }
                
                if (_results[index].collider.TryGetComponent<IClickable>(out var clickable))
                {
                    clickable.OnClick();
                }
            }
        }
    }
}