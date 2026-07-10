using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Entity
{
    [DisallowMultipleComponent]
    public class MapNavigationManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Transform navigationPlane;
        [SerializeField] private LayerMask navigationPlaneLayerMask = ~0;

        [Header("Edge Pan")]
        [SerializeField, Range(0.01f, 0.2f)] private float edgeBoundaryNormalized = 0.03f;
        [SerializeField] private float edgePanSpeed = 20f;

        [Header("Middle Drag Pan")]
        [SerializeField] private float middleDragPanSpeed = 1f;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 12f;
        [SerializeField] private float minOrthographicSize = 8f;
        [SerializeField] private float maxOrthographicSize = 60f;
        [SerializeField] private float minPerspectiveDistance = 12f;
        [SerializeField] private float maxPerspectiveDistance = 120f;

        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 120f;

        private @RTS_InputActions inputActions;
        private bool isMiddleDragging;
        private Vector3 dragStartWorldPoint;

        private void OnEnable()
        {
            inputActions = new @RTS_InputActions();
            inputActions.RTS.Enable();
        }

        private void OnDisable()
        {
            if (inputActions == null)
            {
                return;
            }

            inputActions.RTS.Disable();
            inputActions.Dispose();
            inputActions = null;
            isMiddleDragging = false;
        }

        private void Update()
        {
            var cameraRef = ResolveCamera();
            if (cameraRef == null || inputActions == null)
            {
                return;
            }

            var mouseScreenPosition = GetMouseScreenPosition();

            HandleMiddleMouseDragPan(cameraRef, mouseScreenPosition);
            HandleEdgePan(cameraRef, mouseScreenPosition);
            HandleZoomOrRotation(cameraRef);
            HandleKeyboardRotation(cameraRef);
        }

        private Camera ResolveCamera()
        {
            if (worldCamera != null)
            {
                return worldCamera;
            }

            if (Camera.main != null)
            {
                return Camera.main;
            }

            return null;
        }

        private Vector2 GetMouseScreenPosition()
        {
            if (inputActions != null)
            {
                return inputActions.RTS.MousePosition.ReadValue<Vector2>();
            }

            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }

            return Vector2.zero;
        }

        private void HandleEdgePan(Camera cameraRef, Vector2 mouseScreenPosition)
        {
            if (isMiddleDragging)
            {
                return;
            }

            if (!IsCursorNearScreenEdge(mouseScreenPosition))
            {
                return;
            }

            var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var centeredMouseDirection = (mouseScreenPosition - screenCenter).normalized;
            var worldDirection = ScreenDirectionToNavigationDirection(cameraRef, centeredMouseDirection);

            cameraRef.transform.position += worldDirection * (edgePanSpeed * Time.deltaTime);
        }

        private bool IsCursorNearScreenEdge(Vector2 mouseScreenPosition)
        {
            if (Screen.width <= 0 || Screen.height <= 0)
            {
                return false;
            }

            var left = Screen.width * edgeBoundaryNormalized;
            var right = Screen.width * (1f - edgeBoundaryNormalized);
            var bottom = Screen.height * edgeBoundaryNormalized;
            var top = Screen.height * (1f - edgeBoundaryNormalized);

            return mouseScreenPosition.x <= left
                   || mouseScreenPosition.x >= right
                   || mouseScreenPosition.y <= bottom
                   || mouseScreenPosition.y >= top;
        }

        private Vector3 ScreenDirectionToNavigationDirection(Camera cameraRef, Vector2 screenDirection)
        {
            var planeUp = GetNavigationUp();
            var right = Vector3.ProjectOnPlane(cameraRef.transform.right, planeUp).normalized;
            var forward = Vector3.ProjectOnPlane(cameraRef.transform.forward, planeUp).normalized;

            if (right.sqrMagnitude < 0.0001f)
            {
                right = Vector3.right;
            }

            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = Vector3.forward;
            }

            return (right * screenDirection.x + forward * screenDirection.y).normalized;
        }

        private void HandleMiddleMouseDragPan(Camera cameraRef, Vector2 mouseScreenPosition)
        {
            if (Mouse.current == null)
            {
                return;
            }

            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                isMiddleDragging = TryGetNavigationPoint(cameraRef, mouseScreenPosition, out dragStartWorldPoint);
            }

            if (Mouse.current.middleButton.wasReleasedThisFrame)
            {
                isMiddleDragging = false;
            }

            if (!isMiddleDragging)
            {
                return;
            }

            if (!TryGetNavigationPoint(cameraRef, mouseScreenPosition, out var currentWorldPoint))
            {
                return;
            }

            var worldDelta = dragStartWorldPoint - currentWorldPoint;
            cameraRef.transform.position += worldDelta * middleDragPanSpeed;
        }

        private void HandleZoomOrRotation(Camera cameraRef)
        {
            var inputValue = inputActions.RTS.ZoomCamera.ReadValue<float>();
            if (Mathf.Abs(inputValue) < 0.0001f)
            {
                return;
            }

            if (IsCtrlPressed())
            {
                RotateAroundNavigationPivot(cameraRef, inputValue * rotationSpeed * Time.deltaTime);
                return;
            }

            ApplyZoom(cameraRef, inputValue);
        }

        private void ApplyZoom(Camera cameraRef, float zoomInput)
        {
            if (cameraRef.orthographic)
            {
                var nextSize = cameraRef.orthographicSize - (zoomInput * zoomSpeed * Time.deltaTime);
                cameraRef.orthographicSize = Mathf.Clamp(nextSize, minOrthographicSize, maxOrthographicSize);
                return;
            }

            var pivot = GetNavigationPivot();
            var currentDistance = Vector3.Distance(cameraRef.transform.position, pivot);
            var targetDistance = Mathf.Clamp(currentDistance - (zoomInput * zoomSpeed * Time.deltaTime), minPerspectiveDistance, maxPerspectiveDistance);
            var deltaDistance = currentDistance - targetDistance;
            cameraRef.transform.position += cameraRef.transform.forward * deltaDistance;
        }

        private void HandleKeyboardRotation(Camera cameraRef)
        {
            var rotationInput = inputActions.RTS.RotateCamera.ReadValue<float>();
            if (Mathf.Abs(rotationInput) < 0.0001f)
            {
                return;
            }

            RotateAroundNavigationPivot(cameraRef, rotationInput * rotationSpeed * Time.deltaTime);
        }

        private void RotateAroundNavigationPivot(Camera cameraRef, float angleDelta)
        {
            if (cameraRef == null)
            {
                return;
            }

            var pivot = GetNavigationPivot();
            var axis = GetNavigationUp();
            cameraRef.transform.RotateAround(pivot, axis, angleDelta);
        }

        private bool IsCtrlPressed()
        {
            if (Keyboard.current == null)
            {
                return false;
            }

            return Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
        }

        private Vector3 GetNavigationPivot()
        {
            if (navigationPlane != null)
            {
                return navigationPlane.position;
            }

            var cameraRef = ResolveCamera();
            return cameraRef != null ? cameraRef.transform.position : Vector3.zero;
        }

        private Vector3 GetNavigationUp()
        {
            if (navigationPlane != null)
            {
                return navigationPlane.up;
            }

            return Vector3.up;
        }

        private bool TryGetNavigationPoint(Camera cameraRef, Vector2 screenPosition, out Vector3 pointOnPlane)
        {
            pointOnPlane = default;

            if ((navigationPlaneLayerMask.value != 0)
                && Physics.Raycast(cameraRef.ScreenPointToRay(screenPosition), out var hit, Mathf.Infinity, navigationPlaneLayerMask))
            {
                pointOnPlane = hit.point;
                return true;
            }

            if (navigationPlane == null)
            {
                return false;
            }

            var plane = new Plane(navigationPlane.up, navigationPlane.position);
            var ray = cameraRef.ScreenPointToRay(screenPosition);
            if (!plane.Raycast(ray, out var distance))
            {
                return false;
            }

            pointOnPlane = ray.GetPoint(distance);
            return true;
        }
    }
}