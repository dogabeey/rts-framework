using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraController
{
    public class CameraController : MonoBehaviour
    {
        public enum Plane
        {
            XY,
            XZ,
            YZ
        }
        [SerializeField] private CameraController.Plane planeToMoveOn;  
        [Header("Edge Scrolling Settings")]
        [SerializeField] private float edgeScrollSpeed = 1.0f;
        [SerializeField] private float minScrollThreshold = 1.0f;
        [SerializeField] private float edgeBoundary = 0.03f;
        [Header("Input Scroll Settings")]
        [SerializeField] private float inputScrollSpeed = 1.0f;

        private InputSystem_Actions inputActions;


        private void Start()
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.Enable();
        }

        private void OnDestroy()
        {
            if (inputActions != null)
            {
                inputActions.Player.Disable();
                inputActions.Dispose();
            }
        }

        private bool IsCursorNearEdge()
        {
            Vector2 mousePosition = Input.mousePosition;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            return mousePosition.x < screenWidth * edgeBoundary ||
                   mousePosition.x > screenWidth * (1 - edgeBoundary) ||
                   mousePosition.y < screenHeight * edgeBoundary ||
                   mousePosition.y > screenHeight * (1 - edgeBoundary);
        }

        /// <summary>
        /// Calculates the scroll direction based on the mouse position relative to the screen edges. If the mouse is near an edge, it returns a normalized vector from screen's mid-point to cursor location.
        /// </summary>
        /// <returns></returns>
        private Vector3 CalculateScrollDirection()
        {
            if(IsCursorNearEdge())
            {
                Vector2 scrollDirection = Vector2.zero;
                Vector2 mousePosition = Input.mousePosition;
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;
                // Offset mouse position to be relative to the center of the screen
                Vector2 offsetedMousePos = new Vector2(mousePosition.x - screenWidth / 2, mousePosition.y - screenHeight / 2);
                Vector2 normalizedOffset = offsetedMousePos.normalized;

                Vector3 camMovementVector = Vector3.zero;

                switch (planeToMoveOn)
                {
                    case Plane.XY:
                        camMovementVector = new Vector3(normalizedOffset.x, normalizedOffset.y, 0);
                        break;
                    case Plane.XZ:
                        camMovementVector = new Vector3(normalizedOffset.x, 0, normalizedOffset.y);
                        break;
                    case Plane.YZ:
                        camMovementVector = new Vector3(0, normalizedOffset.x, normalizedOffset.y);
                        break;
                    default:
                        break;
                }

                Vector3 thresholdedMovement = new Vector3(Mathf.Abs(camMovementVector.x) > minScrollThreshold ? camMovementVector.x : 0,
                                                          Mathf.Abs(camMovementVector.y) > minScrollThreshold ? camMovementVector.y : 0,
                                                          Mathf.Abs(camMovementVector.z) > minScrollThreshold ? camMovementVector.z : 0);
                Vector3 finalMovement = thresholdedMovement * edgeScrollSpeed;
                return finalMovement;
            }
            return Vector3.zero;
        }

        private Vector3 CalculateLookInputDirection()
        {
            if (inputActions == null)
            {
                return Vector3.zero;
            }

            Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

            switch (planeToMoveOn)
            {
                case Plane.XY:
                    return new Vector3(lookInput.x, lookInput.y, 0f) * inputScrollSpeed;
                case Plane.XZ:
                    return new Vector3(lookInput.x, 0f, lookInput.y) * inputScrollSpeed;
                case Plane.YZ:
                    return new Vector3(0f, lookInput.x, lookInput.y) * inputScrollSpeed;
                default:
                    return Vector3.zero;
            }
        }

        private void HandleCameraMovement()
        {
            Vector3 scrollDirection = CalculateScrollDirection() + CalculateLookInputDirection();
            transform.position += scrollDirection * Time.deltaTime;
        }

        private void Update()
        {
            HandleCameraMovement();
        }
    }
}
