using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Entity
{
    [DisallowMultipleComponent]
    public class SelectionManager : MonoBehaviour, @RTS_InputActions.IRTSActions
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private LayerMask selectableLayers = ~0;
        [SerializeField] private float dragSelectionThresholdPixels = 8f;

        private @RTS_InputActions inputActions;
        private Vector2 pressStartScreenPosition;
        private bool isDraggingSelection;

        private void OnEnable()
        {
            inputActions = new @RTS_InputActions();
            inputActions.RTS.AddCallbacks(this);
            inputActions.RTS.Enable();
        }

        private void OnDisable()
        {
            if (inputActions == null)
            {
                return;
            }

            inputActions.RTS.RemoveCallbacks(this);
            inputActions.RTS.Disable();
            inputActions.Dispose();
            inputActions = null;
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

        private bool IsAdditiveSelectionActive()
        {
            return inputActions != null && inputActions.RTS.AddToSelection.IsPressed();
        }

        private Vector2 GetCurrentMouseScreenPosition(InputAction.CallbackContext context)
        {
            if (inputActions != null)
            {
                return inputActions.RTS.MousePosition.ReadValue<Vector2>();
            }

            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }

            return context.ReadValue<Vector2>();
        }

        private void PerformSingleSelection(Vector2 screenPosition, bool additive)
        {
            var cameraRef = ResolveCamera();
            if (cameraRef == null)
            {
                return;
            }

            var ray = cameraRef.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, selectableLayers))
            {
                if (!additive)
                {
                    SelectableComponent.DeselectAll();
                }
                return;
            }

            var clickedSelectable = hit.collider.GetComponentInParent<SelectableComponent>();
            if (clickedSelectable == null)
            {
                if (!additive)
                {
                    SelectableComponent.DeselectAll();
                }
                return;
            }

            if (!additive)
            {
                SelectableComponent.DeselectAll();
            }

            clickedSelectable.SetSelected(true);
        }

        private void PerformMassSelection(Vector2 startScreenPosition, Vector2 endScreenPosition, bool additive)
        {
            var cameraRef = ResolveCamera();
            if (cameraRef == null)
            {
                return;
            }

            if (!additive)
            {
                SelectableComponent.DeselectAll();
            }

            var selectionRect = CreateScreenRect(startScreenPosition, endScreenPosition);
            foreach (var selectable in SelectableComponent.All)
            {
                if (selectable == null)
                {
                    continue;
                }

                var viewportPoint = cameraRef.WorldToScreenPoint(selectable.transform.position);
                if (viewportPoint.z < 0f)
                {
                    continue;
                }

                if (selectionRect.Contains(new Vector2(viewportPoint.x, viewportPoint.y)))
                {
                    selectable.SetSelected(true);
                }
            }
        }

        private static Rect CreateScreenRect(Vector2 p1, Vector2 p2)
        {
            var min = Vector2.Min(p1, p2);
            var max = Vector2.Max(p1, p2);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        public void OnPanCamera(InputAction.CallbackContext context)
        {
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
        }

        public void OnZoomCamera(InputAction.CallbackContext context)
        {
        }

        public void OnRotateCamera(InputAction.CallbackContext context)
        {
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                pressStartScreenPosition = GetCurrentMouseScreenPosition(context);
                isDraggingSelection = false;
                return;
            }

            if (!context.canceled)
            {
                return;
            }

            if (isDraggingSelection)
            {
                return;
            }

            var additive = IsAdditiveSelectionActive();
            PerformSingleSelection(GetCurrentMouseScreenPosition(context), additive);
        }

        public void OnAddToSelection(InputAction.CallbackContext context)
        {
        }

        public void OnCommandMove(InputAction.CallbackContext context)
        {
        }

        public void OnCommandAttackMove(InputAction.CallbackContext context)
        {
        }

        public void OnStopCommand(InputAction.CallbackContext context)
        {
        }

        public void OnHoldPosition(InputAction.CallbackContext context)
        {
        }

        public void OnPatrolCommand(InputAction.CallbackContext context)
        {
        }

        public void OnQueueModifier(InputAction.CallbackContext context)
        {
        }

        public void OnCenterCameraOnSelection(InputAction.CallbackContext context)
        {
        }

        public void OnSelectAllIdleUnits(InputAction.CallbackContext context)
        {
        }

        public void OnSelectControlGroup1(InputAction.CallbackContext context)
        {
        }

        public void OnSelectControlGroup2(InputAction.CallbackContext context)
        {
        }

        public void OnSelectControlGroup3(InputAction.CallbackContext context)
        {
        }

        public void OnSelectControlGroup4(InputAction.CallbackContext context)
        {
        }

        public void OnSelectControlGroup5(InputAction.CallbackContext context)
        {
        }

        public void OnSetControlGroup1(InputAction.CallbackContext context)
        {
        }

        public void OnSetControlGroup2(InputAction.CallbackContext context)
        {
        }

        public void OnSetControlGroup3(InputAction.CallbackContext context)
        {
        }

        public void OnSetControlGroup4(InputAction.CallbackContext context)
        {
        }

        public void OnSetControlGroup5(InputAction.CallbackContext context)
        {
        }

        public void OnPauseGame(InputAction.CallbackContext context)
        {
        }

        public void OnQuickSave(InputAction.CallbackContext context)
        {
        }

        public void OnQuickLoad(InputAction.CallbackContext context)
        {
        }

        public void OnSelectArea(InputAction.CallbackContext context)
        {
            var currentMousePosition = GetCurrentMouseScreenPosition(context);

            if (context.started)
            {
                pressStartScreenPosition = currentMousePosition;
                isDraggingSelection = false;
                return;
            }

            if (context.performed)
            {
                var dragDistance = Vector2.Distance(pressStartScreenPosition, currentMousePosition);
                isDraggingSelection = dragDistance >= dragSelectionThresholdPixels;
                return;
            }

            if (!context.canceled)
            {
                return;
            }

            var endMousePosition = currentMousePosition;
            var additive = IsAdditiveSelectionActive();
            var dragDistanceOnRelease = Vector2.Distance(pressStartScreenPosition, endMousePosition);
            var shouldUseMassSelection = isDraggingSelection || dragDistanceOnRelease >= dragSelectionThresholdPixels;
            if (shouldUseMassSelection)
            {
                PerformMassSelection(pressStartScreenPosition, endMousePosition, additive);
            }

            isDraggingSelection = false;
        }
    }
}
