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
        [SerializeField] private Vector2 _panLimitX = new(-10f, 10f);
        [SerializeField] private Vector2 _panLimitY = new(-10f, 10f);

        // Touch-specific scaling for pinch sensitivity
        [SerializeField] private float _pinchZoomFactor = 0.01f;

        private UnityEngine.Camera _cam;
        private Vector3 _dragOrigin;
        private bool _isDragging;
        private string _cameraDataPath;

        private void Start()
        {
            Application.targetFrameRate = 120;
            _cam = GetComponent<UnityEngine.Camera>();
            ChangePanOffsetWithZoom();
        }

        private void Update()
        {
            // Mobile touches take priority
            if (Input.touchCount > 0)
            {
                HandleTouchInput();
                return;
            }

            // Desktop controls
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

                ClampCameraPosition();
            }
        }

        private void HandlePan()
        {
            if (!_enablePan) return;

            if (Input.GetMouseButtonDown(1))
            {
                _dragOrigin = _cam.ScreenToWorldPoint(Input.mousePosition);
                _isDragging = true;
            }

            if (Input.GetMouseButton(1) && _isDragging)
            {
                Vector3 difference = _dragOrigin - _cam.ScreenToWorldPoint(Input.mousePosition);
                transform.position += difference * _panSpeed;
                ClampCameraPosition();
            }

            if (Input.GetMouseButtonUp(1))
            {
                _isDragging = false;
            }
        }

        private void HandleTouchInput()
        {
            if (!_enablePan && !_zoomToMouse) return;

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    _dragOrigin = _cam.ScreenToWorldPoint(touch.position);
                    _isDragging = true;
                }
                else if (touch.phase == TouchPhase.Moved && _isDragging)
                {
                    Vector3 difference = _dragOrigin - _cam.ScreenToWorldPoint(touch.position);
                    transform.position += difference * _panSpeed;
                    ClampCameraPosition();
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    _isDragging = false;
                }
            }
            else if (Input.touchCount >= 2)
            {
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);

                // Previous positions
                Vector2 prevPos0 = t0.position - t0.deltaPosition;
                Vector2 prevPos1 = t1.position - t1.deltaPosition;

                // Distances for pinch detection
                float prevDistance = (prevPos0 - prevPos1).magnitude;
                float currDistance = (t0.position - t1.position).magnitude;
                float delta = prevDistance - currDistance;

                // Midpoints for panning
                Vector2 prevMidScreen = (prevPos0 + prevPos1) * 0.5f;
                Vector2 currMidScreen = (t0.position + t1.position) * 0.5f;

                // Handle zoom (pinch)
                if (Mathf.Abs(delta) > 0.0f)
                {
                    // Save world midpoint before zoom if we want to keep it stable
                    Vector3 midWorldBeforeZoom = _cam.ScreenToWorldPoint(new Vector3(prevMidScreen.x, prevMidScreen.y, 0f));

                    float newSize = _cam.orthographicSize + delta * _zoomSpeed * _pinchZoomFactor;
                    _cam.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);

                    // If _zoomToMouse is true, keep midpoint stable
                    if (_zoomToMouse)
                    {
                        Vector3 midWorldAfterZoom = _cam.ScreenToWorldPoint(new Vector3(currMidScreen.x, currMidScreen.y, 0f));
                        Vector3 zoomOffset = midWorldBeforeZoom - midWorldAfterZoom;
                        transform.position += zoomOffset;
                    }

                    ChangePanOffsetWithZoom();
                }

                // Handle two-finger pan (move midpoint)
                Vector3 prevMidWorld = _cam.ScreenToWorldPoint(new Vector3(prevMidScreen.x, prevMidScreen.y, 0f));
                Vector3 currMidWorld = _cam.ScreenToWorldPoint(new Vector3(currMidScreen.x, currMidScreen.y, 0f));
                Vector3 midDeltaWorld = prevMidWorld - currMidWorld;

                // Apply pan from midpoint movement
                transform.position += midDeltaWorld * _panSpeed;

                ChangePanOffsetWithZoom();
                ClampCameraPosition();
            }
        }

        private void ZoomToMousePoint(float scrollDelta)
        {
            Vector3 mouseWorldPosBeforeZoom = _cam.ScreenToWorldPoint(Input.mousePosition);

            float newSize = _cam.orthographicSize - scrollDelta * _zoomSpeed;
            _cam.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);

            Vector3 mouseWorldPosAfterZoom = _cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 zoomOffset = mouseWorldPosBeforeZoom - mouseWorldPosAfterZoom;
            transform.position += zoomOffset;
            ChangePanOffsetWithZoom();
            ClampCameraPosition();
        }

        private void ChangePanOffsetWithZoom()
        {
            if (_cam == null) _cam = GetComponent<UnityEngine.Camera>();
            _panOffset = _cam.orthographicSize / 2f;
        }

        private void SimpleZoom(float scrollDelta)
        {
            float newSize = _cam.orthographicSize - scrollDelta * _zoomSpeed;
            _cam.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);
            ClampCameraPosition();
        }

        private void ClampCameraPosition()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, _panLimitX.x + _panOffset, _panLimitX.y - _panOffset);
            pos.y = Mathf.Clamp(pos.y, _panLimitY.x + _panOffset, _panLimitY.y - _panOffset);
            transform.position = pos;
        }

        private void OnDrawGizmos()
        {
            if (_cam == null) _cam = GetComponent<UnityEngine.Camera>();

            if (_zoomToMouse && Application.isPlaying)
            {
                Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(mousePos, 0.5f);
            }

            Gizmos.color = Color.blue;
            Vector3 center = new Vector3(
                (_panLimitX.x + _panLimitX.y) * 0.5f,
                (_panLimitY.x + _panLimitY.y) * 0.5f,
                0f
            );

            Vector3 size = new Vector3(
                _panLimitX.y - _panLimitX.x - _panOffset * 2f,
                _panLimitY.y - _panLimitY.x - _panOffset * 2f,
                0f
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}