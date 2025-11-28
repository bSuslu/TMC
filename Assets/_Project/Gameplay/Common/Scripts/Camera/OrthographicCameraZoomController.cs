using UnityEngine;

namespace TMC._Project.Gameplay.Common.Scripts.Camera
{
    public class OrthographicCameraZoomController : MonoBehaviour
    {
        [Header("Zoom Settings")] 
        
        [SerializeField] private float _zoomSpeed = 5f;
        [SerializeField] private float _minZoom = 2f;
        [SerializeField] private float _maxZoom = 10f;
        [SerializeField] private bool _zoomToMouse = true;
        
        [Header("Pan Settings")] 
        
        [SerializeField] private float _panSpeed = 1f;
        [SerializeField] private float _panOffset = 1f;
        [SerializeField] private bool _enablePan = true;
        [SerializeField] private float _panSkillEdgeBuffer = 1f;
        [SerializeField] private Vector2 _panLimitX = new(-10f, 10f);
        [SerializeField] private Vector2 _panLimitY = new(-10f, 10f);

        private UnityEngine.Camera _cam;
        private Vector3 _dragOrigin;
        private bool _isDragging;
        private string _cameraDataPath;

        private void Start()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            ChangePanOffsetWithZoom();
        }

        private void Update()
        {
            HandleZoom();
            HandlePan();
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                if (_zoomToMouse)
                {
                    ZoomToMousePoint(scroll);
                }
                else
                {
                    SimpleZoom(scroll);
                }

                // Enforce position limits after zoom
                ClampCameraPosition();
            }
        }

        private void HandlePan()
        {
            if (!_enablePan) return;

            // Drag using right mouse button
            if (Input.GetMouseButtonDown(1)) // Right click
            {
                _dragOrigin = _cam.ScreenToWorldPoint(Input.mousePosition);
                _isDragging = true;
            }

            if (Input.GetMouseButton(1) && _isDragging)
            {
                Vector3 difference = _dragOrigin - _cam.ScreenToWorldPoint(Input.mousePosition);
                transform.position += difference * _panSpeed;

                // Enforce movement limits
                ClampCameraPosition();
            }

            if (Input.GetMouseButtonUp(1))
            {
                _isDragging = false;
            }
        }

        private void ZoomToMousePoint(float scrollDelta)
        {
            // Capture mouse position in world space before zoom
            Vector3 mouseWorldPosBeforeZoom = _cam.ScreenToWorldPoint(Input.mousePosition);

            // Apply zoom
            float newSize = _cam.orthographicSize - scrollDelta * _zoomSpeed;
            _cam.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);

            // Capture mouse world position after zoom
            Vector3 mouseWorldPosAfterZoom = _cam.ScreenToWorldPoint(Input.mousePosition);

            // Shift camera to keep mouse point stable
            Vector3 zoomOffset = mouseWorldPosBeforeZoom - mouseWorldPosAfterZoom;
            transform.position += zoomOffset;
            ChangePanOffsetWithZoom();
            // Enforce position limits after zoom
            ClampCameraPosition();
        }

        private void ChangePanOffsetWithZoom()
        {
            _panOffset = _cam.orthographicSize / 2f;
        }

        private void SimpleZoom(float scrollDelta)
        {
            float newSize = _cam.orthographicSize - scrollDelta * _zoomSpeed;
            _cam.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);

            // Enforce position limits after zoom
            ClampCameraPosition();
        }

        private void ClampCameraPosition()
        {
            // Clamp camera position within defined pan limits
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, _panLimitX.x + _panOffset, _panLimitX.y - _panOffset);
            pos.y = Mathf.Clamp(pos.y, _panLimitY.x + _panOffset, _panLimitY.y - _panOffset);
            transform.position = pos;
        }

        private void OnDrawGizmos()
        {
            // Visualize zoom center during play mode
            if (_zoomToMouse && Application.isPlaying)
            {
                Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(mousePos, 0.5f);
            }

            // Visualize pan boundaries
            Gizmos.color = Color.blue;
            Vector3 center = new Vector3(
                (_panLimitX.x + _panLimitX.y) * 0.5f,
                (_panLimitY.x + _panLimitY.y) * 0.5f,
                0f
            );
            Vector3 size = new Vector3(
                _panLimitX.y - _panOffset - _panLimitX.x - _panOffset,
                _panLimitY.y - _panOffset - _panLimitY.x - _panOffset,
                0f
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}