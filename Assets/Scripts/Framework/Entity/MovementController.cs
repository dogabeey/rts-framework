using System.Collections.Generic;
using Game.ModifierSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entity
{
    public class MovementController : MonoBehaviour
    {
        public MovementTags MovementTagList => movementTagList;
        public float Speed => baseSpeed;
        public float Acceleration => baseAcceleration;
        public float Deceleration => baseDeceleration;
        public bool UseFormationPriority => useFormationPriority;
        public int FormationPriority => formationPriority;
        public bool HasMoveTarget => hasMoveTarget;
        public Vector3 CurrentMoveTarget => currentMoveTarget;

        [SerializeField, FoldoutGroup("Movement")] private MovementTags movementTagList;
        [SerializeField, FoldoutGroup("Movement")] private float baseSpeed;
        [SerializeField, FoldoutGroup("Movement")] private float baseAcceleration;
        [SerializeField, FoldoutGroup("Movement")] private float baseDeceleration;
        [SerializeField, FoldoutGroup("Movement")] private float stoppingDistance = 0.1f;
        [SerializeField, FoldoutGroup("Movement")] private float turnSpeed = 360f;
        [SerializeField, FoldoutGroup("Formation")] private bool useFormationPriority = true;
        [SerializeField, FoldoutGroup("Formation")] private int formationPriority;
        [SerializeField, FoldoutGroup("Modifier")] private List<Modifier> movementModifiers = new List<Modifier>();

        private bool hasMoveTarget;
        private Vector3 currentMoveTarget;
        private float currentSpeed;

        private void Update()
        {
            TickMovement(Time.deltaTime);
        }

        public void Move(float deltaTime, Vector2 targetPosition)
        {
            SetMoveTarget(new Vector3(targetPosition.x, transform.position.y, targetPosition.y));
        }

        public void SetMoveTarget(Vector3 worldTarget)
        {
            currentMoveTarget = worldTarget;
            hasMoveTarget = true;
        }

        public void Stop()
        {
            hasMoveTarget = false;
            currentSpeed = 0f;
        }

        private void TickMovement(float deltaTime)
        {
            if (!hasMoveTarget)
            {
                return;
            }

            var toTarget = currentMoveTarget - transform.position;
            var planarToTarget = new Vector3(toTarget.x, 0f, toTarget.z);
            var remainingDistance = planarToTarget.magnitude;
            if (remainingDistance <= stoppingDistance)
            {
                transform.position = new Vector3(currentMoveTarget.x, transform.position.y, currentMoveTarget.z);
                Stop();
                return;
            }

            var desiredSpeed = Mathf.Max(baseSpeed, 0f);
            if (currentSpeed < desiredSpeed)
            {
                currentSpeed = Mathf.Min(desiredSpeed, currentSpeed + Mathf.Max(baseAcceleration, 0f) * deltaTime);
            }
            else
            {
                currentSpeed = Mathf.Max(desiredSpeed, currentSpeed - Mathf.Max(baseDeceleration, 0f) * deltaTime);
            }

            var moveDirection = planarToTarget / remainingDistance;
            var step = Mathf.Min(remainingDistance, currentSpeed * deltaTime);
            transform.position += moveDirection * step;

            if (moveDirection.sqrMagnitude > 0.0001f)
            {
                var targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Mathf.Max(turnSpeed, 0f) * deltaTime);
            }
        }

        [System.Flags]
        public enum MovementTags
        {
            None = 0,
            Ground = 1 << 0,
            Air = 1 << 1,
            Water = 1 << 2,
            // Add more movement tags as needed
        }
    }
}
