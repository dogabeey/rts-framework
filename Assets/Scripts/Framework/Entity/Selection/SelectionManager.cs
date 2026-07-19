using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Entity
{
    public enum SelectionBoxMode
    {
        Sprite,
        ProjectionCube
    }

    [DisallowMultipleComponent]
    public class SelectionManager : MonoBehaviour, @RTS_InputActions.IRTSActions
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Transform selectionPlane;
        [SerializeField] private LayerMask selectionPlaneLayerMask = ~0;
        [SerializeField] private MovementManager movementManager;

        [Header("Selection Box Visual")]
        [SerializeField] private SelectionBoxMode selectionBoxMode = SelectionBoxMode.Sprite;

        [Header("Sprite Mode")]
        [SerializeField] private SpriteRenderer selectionBoxRenderer;

        [Header("Projection Cube Mode")]
        [SerializeField] private Transform selectionBoxProjector;
        [SerializeField] private float projectionDepth = 2f;

        [Header("Shared")]
        [SerializeField] private float selectionBoxPlaneOffset = 0.02f;
        [SerializeField] private float singleSelectionRadiusPixels = 40f;
        [SerializeField] private float dragSelectionThresholdPixels = 8f;

        private @RTS_InputActions inputActions;
        private Vector2 pressStartScreenPosition;
        private bool isPointerDown;
        private bool isDraggingSelection;

        private readonly List<UnitController> selectedUnitsBuffer = new List<UnitController>();

        private void Awake()
        {
            ConfigureSelectionBoxRenderer();
            SetSelectionBoxVisible(false);
            ResolveMovementManager();
        }

        private void OnEnable()
        {
            ConfigureSelectionBoxRenderer();
            SetSelectionBoxVisible(false);
            ResolveMovementManager();

            inputActions = new @RTS_InputActions();
            inputActions.RTS.AddCallbacks(this);
            inputActions.RTS.Enable();
        }

        private void Update()
        {
            if (!isPointerDown)
            {
                return;
            }

            var currentMousePosition = GetCurrentMouseScreenPosition();
            var dragDistance = Vector2.Distance(pressStartScreenPosition, currentMousePosition);
            if (!isDraggingSelection && dragDistance >= dragSelectionThresholdPixels)
            {
                isDraggingSelection = true;
                SetSelectionBoxVisible(true);
            }

            if (isDraggingSelection)
            {
                UpdateSelectionBoxVisual(pressStartScreenPosition, currentMousePosition);
            }
        }

        private void OnDisable()
        {
            SetSelectionBoxVisible(false);
            isPointerDown = false;
            isDraggingSelection = false;

            if (inputActions == null)
            {
                return;
            }

            inputActions.RTS.RemoveCallbacks(this);
            inputActions.RTS.Disable();
            inputActions.Dispose();
            inputActions = null;
        }

        private void ConfigureSelectionBoxRenderer()
        {
            if (selectionBoxMode == SelectionBoxMode.Sprite && selectionBoxRenderer != null)
            {
                selectionBoxRenderer.drawMode = SpriteDrawMode.Sliced;
                selectionBoxRenderer.size = Vector2.one;
                selectionBoxRenderer.transform.localScale = Vector3.one;
            }
        }

        private void ResolveMovementManager()
        {
            if (movementManager != null)
            {
                return;
            }

            movementManager = FindFirstObjectByType<MovementManager>();
        }

        private void SetSelectionBoxVisible(bool isVisible)
        {
            switch (selectionBoxMode)
            {
                case SelectionBoxMode.Sprite:
                    if (selectionBoxRenderer != null)
                    {
                        var go = selectionBoxRenderer.gameObject;
                        if (go.activeSelf != isVisible) go.SetActive(isVisible);
                    }
                    break;

                case SelectionBoxMode.ProjectionCube:
                    if (selectionBoxProjector != null)
                    {
                        if (selectionBoxProjector.gameObject.activeSelf != isVisible)
                            selectionBoxProjector.gameObject.SetActive(isVisible);
                    }
                    break;
            }
        }

        private void UpdateSelectionBoxVisual(Vector2 startScreen, Vector2 endScreen)
        {
            if (selectionPlane == null)
            {
                return;
            }

            var minScreen = Vector2.Min(startScreen, endScreen);
            var maxScreen = Vector2.Max(startScreen, endScreen);

            // Project all 4 screen-rect corners to the world plane
            if (!TryGetPlanePoint(new Vector2(minScreen.x, minScreen.y), out var wBL)) return;
            if (!TryGetPlanePoint(new Vector2(maxScreen.x, minScreen.y), out var wBR)) return;
            if (!TryGetPlanePoint(new Vector2(maxScreen.x, maxScreen.y), out var wTR)) return;
            if (!TryGetPlanePoint(new Vector2(minScreen.x, maxScreen.y), out var wTL)) return;

            var center = (wBL + wBR + wTR + wTL) * 0.25f + selectionPlane.up * selectionBoxPlaneOffset;

            // Width and height are real world-edge lengths averaged across opposing edges
            var width  = Mathf.Max((Vector3.Distance(wBL, wBR) + Vector3.Distance(wTL, wTR)) * 0.5f, 0.001f);
            var height = Mathf.Max((Vector3.Distance(wBL, wTL) + Vector3.Distance(wBR, wTR)) * 0.5f, 0.001f);

            // Build stable local axes from projected drag edges so visuals track camera rotation.
            var planeNormal = selectionPlane.up.normalized;
            var avgHorizontal = ((wBR - wBL) + (wTR - wTL)) * 0.5f;
            var avgVertical = ((wTL - wBL) + (wTR - wBR)) * 0.5f;

            var upAxis = Vector3.ProjectOnPlane(avgVertical, planeNormal).normalized;
            if (upAxis.sqrMagnitude < 0.0001f)
            {
                var cam = ResolveCamera();
                var fallback = cam != null ? cam.transform.up : selectionPlane.forward;
                upAxis = Vector3.ProjectOnPlane(fallback, planeNormal).normalized;
                if (upAxis.sqrMagnitude < 0.0001f)
                {
                    upAxis = Vector3.ProjectOnPlane(selectionPlane.forward, planeNormal).normalized;
                }
            }

            var rightAxis = Vector3.Cross(planeNormal, upAxis).normalized;
            if (Vector3.Dot(rightAxis, avgHorizontal) < 0f)
            {
                rightAxis = -rightAxis;
                upAxis = -upAxis;
            }

            // Sprite local axes: X=rightAxis, Y=upAxis, Z=planeNormal
            var spriteRotation = Quaternion.LookRotation(Vector3.Cross(rightAxis, upAxis), upAxis);
            // Projection cube local axes: X=rightAxis, Y=planeNormal, Z=upAxis
            var projectorRotation = Quaternion.LookRotation(upAxis, planeNormal);

            switch (selectionBoxMode)
            {
                case SelectionBoxMode.Sprite:
                    if (selectionBoxRenderer == null) return;
                    selectionBoxRenderer.transform.position = center;
                    selectionBoxRenderer.transform.rotation = spriteRotation;
                    selectionBoxRenderer.transform.localScale = Vector3.one;
                    selectionBoxRenderer.size = new Vector2(width, height);
                    break;

                case SelectionBoxMode.ProjectionCube:
                    if (selectionBoxProjector == null) return;
                    selectionBoxProjector.position = center;
                    selectionBoxProjector.rotation = projectorRotation;
                    // X = width on plane, Z = height on plane, Y = projection depth
                    selectionBoxProjector.localScale = new Vector3(width, Mathf.Max(projectionDepth, 0.001f), height);
                    break;
            }
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

        private bool TryGetPlanePoint(Vector2 screenPosition, out Vector3 pointOnPlane)
        {
            pointOnPlane = default;

            var cameraRef = ResolveCamera();
            if (cameraRef == null)
            {
                return false;
            }

            if ((selectionPlaneLayerMask.value != 0) && Physics.Raycast(cameraRef.ScreenPointToRay(screenPosition), out var hit, Mathf.Infinity, selectionPlaneLayerMask))
            {
                pointOnPlane = hit.point;
                return true;
            }

            if (selectionPlane == null)
            {
                return false;
            }

            var plane = new Plane(selectionPlane.up, selectionPlane.position);
            var ray = cameraRef.ScreenPointToRay(screenPosition);
            if (!plane.Raycast(ray, out var distance))
            {
                return false;
            }

            pointOnPlane = ray.GetPoint(distance);
            return true;
        }

        private bool IsAdditiveSelectionActive()
        {
            return inputActions != null && inputActions.RTS.AddToSelection.IsPressed();
        }

        private bool IsQueueModifierActive()
        {
            return inputActions != null && inputActions.RTS.QueueModifier.IsPressed();
        }

        private void CollectSelectedUnits()
        {
            selectedUnitsBuffer.Clear();
            foreach (var selectable in SelectableComponent.All)
            {
                if (selectable != null && selectable.IsSelected
                    && selectable.TryGetComponent<UnitController>(out var unitController)
                    && unitController.movementController != null)
                {
                    selectedUnitsBuffer.Add(unitController);
                }
            }
        }

        private bool TryGetSelectedUnits(out IReadOnlyList<UnitController> units)
        {
            CollectSelectedUnits();
            units = selectedUnitsBuffer;
            return selectedUnitsBuffer.Count > 0;
        }

        private bool TryGetCommandTarget(out Vector3 targetPoint)
        {
            return TryGetPlanePoint(GetCurrentMouseScreenPosition(), out targetPoint);
        }

        private void SetSelectedUnitsMission(EntityMissionType missionType)
        {
            foreach (var unit in selectedUnitsBuffer)
            {
                unit.SetMissionState(missionType, true);
            }
        }

        private Vector2 GetCurrentMouseScreenPosition()
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

        // Selection operates entirely in screen space.
        // Units are projected with WorldToScreenPoint and tested against the drag rectangle.
        // This is accurate regardless of camera type, plane orientation, or layer masks.

        private void PerformSingleSelection(Vector2 clickScreenPosition, bool additive)
        {
            var cam = ResolveCamera();
            if (cam == null)
            {
                return;
            }

            SelectableComponent best = null;
            var bestDist = float.MaxValue;

            foreach (var selectable in SelectableComponent.All)
            {
                if (selectable == null)
                {
                    continue;
                }

                var sp = cam.WorldToScreenPoint(selectable.transform.position);
                if (sp.z < 0f)
                {
                    continue;
                }

                var dist = Vector2.Distance(new Vector2(sp.x, sp.y), clickScreenPosition);
                if (dist > singleSelectionRadiusPixels || dist >= bestDist)
                {
                    continue;
                }

                bestDist = dist;
                best = selectable;
            }

            if (best == null)
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

            best.SetSelected(true);
        }

        private void PerformMassSelection(Vector2 startScreen, Vector2 endScreen, bool additive)
        {
            if (!additive)
            {
                SelectableComponent.DeselectAll();
            }

            var cam = ResolveCamera();
            if (cam == null)
            {
                return;
            }

            var minX = Mathf.Min(startScreen.x, endScreen.x);
            var maxX = Mathf.Max(startScreen.x, endScreen.x);
            var minY = Mathf.Min(startScreen.y, endScreen.y);
            var maxY = Mathf.Max(startScreen.y, endScreen.y);

            foreach (var selectable in SelectableComponent.All)
            {
                if (selectable == null)
                {
                    continue;
                }

                var sp = cam.WorldToScreenPoint(selectable.transform.position);
                if (sp.z < 0f)
                {
                    continue;
                }

                if (sp.x >= minX && sp.x <= maxX && sp.y >= minY && sp.y <= maxY)
                {
                    selectable.SetSelected(true);
                }
            }
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
                pressStartScreenPosition = GetCurrentMouseScreenPosition();
                isPointerDown = true;
                isDraggingSelection = false;
                SetSelectionBoxVisible(false);
                return;
            }

            if (!context.canceled)
            {
                return;
            }

            if (!isPointerDown)
            {
                return;
            }

            isPointerDown = false;
            var endMousePosition = GetCurrentMouseScreenPosition();
            var additive = IsAdditiveSelectionActive();

            if (isDraggingSelection)
            {
                PerformMassSelection(pressStartScreenPosition, endMousePosition, additive);
            }
            else
            {
                PerformSingleSelection(endMousePosition, additive);
            }

            SetSelectionBoxVisible(false);
            isDraggingSelection = false;
        }

        public void OnAddToSelection(InputAction.CallbackContext context)
        {
        }

        public void OnCommandMove(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            ResolveMovementManager();
            if (movementManager == null)
            {
                return;
            }

            if (!TryGetCommandTarget(out var targetPoint))
            {
                return;
            }

            if (!TryGetSelectedUnits(out var units))
            {
                return;
            }

            var queue = IsQueueModifierActive();
            if (!queue)
            {
                // There is no separate Move mission. Resetting to Idle cancels an
                // attack-move/patrol order before the new move target is issued.
                SetSelectedUnitsMission(EntityMissionType.Idle);
            }

            movementManager.CommandMove(units, targetPoint, queue);
        }

        public void OnCommandAttackMove(InputAction.CallbackContext context)
        {
            if (!context.performed || !TryGetCommandTarget(out var targetPoint) || !TryGetSelectedUnits(out var units))
            {
                return;
            }

            ResolveMovementManager();
            if (movementManager == null)
            {
                return;
            }

            movementManager.CommandMove(units, targetPoint, IsQueueModifierActive());
            SetSelectedUnitsMission(EntityMissionType.AttackMove);
        }

        public void OnStopCommand(InputAction.CallbackContext context)
        {
            if (!context.performed || !TryGetSelectedUnits(out _))
            {
                return;
            }

            SetSelectedUnitsMission(EntityMissionType.Idle);
        }

        public void OnHoldPosition(InputAction.CallbackContext context)
        {
            if (!context.performed || !TryGetSelectedUnits(out _))
            {
                return;
            }

            SetSelectedUnitsMission(EntityMissionType.Guard);
        }

        public void OnPatrolCommand(InputAction.CallbackContext context)
        {
            if (!context.performed || !TryGetCommandTarget(out var targetPoint) || !TryGetSelectedUnits(out var units))
            {
                return;
            }

            ResolveMovementManager();
            if (movementManager == null)
            {
                return;
            }

            movementManager.CommandPatrol(units, targetPoint);
            SetSelectedUnitsMission(EntityMissionType.Patrol);
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
            // Drag lifecycle is driven by Select press/release + Update mouse position polling
            // to avoid Hold interaction timing and canvas mode inconsistencies.
        }
    }
}
