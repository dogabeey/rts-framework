using System.Collections.Generic;
using Game.ModifierSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Entity
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MovementController : MonoBehaviour
    {
        public MovementTags MovementTagList => movementTagList;
        public float Speed => baseSpeed;
        public float Acceleration => baseAcceleration;
        public float Deceleration => baseDeceleration;
        public bool UseFormationPriority => useFormationPriority;
        public int FormationPriority => formationPriority;
        public float FormationSpacing => formationSpacing;
        public int MaxUnitsPerRow => maxUnitsPerRow;
        public bool HasMoveTarget => hasMoveTarget;
        public Vector3 CurrentMoveTarget => currentMoveTarget;

        [ReadOnly] public  EntityController referenceEntity;

        [SerializeField, FoldoutGroup("Movement")] private MovementTags movementTagList;
        [SerializeField, FoldoutGroup("Movement")] private float baseSpeed;
        [SerializeField, FoldoutGroup("Movement")] private float baseAcceleration;
        [SerializeField, FoldoutGroup("Movement")] private float baseDeceleration;
        [SerializeField, FoldoutGroup("Movement")] private float stoppingDistance = 0.1f;
        [SerializeField, FoldoutGroup("Movement")] private float turnSpeed = 360f;
        [SerializeField, FoldoutGroup("Formation")] private bool useFormationPriority = true;
        [SerializeField, FoldoutGroup("Formation")] private int formationPriority;
        [SerializeField, FoldoutGroup("Formation")] private float formationSpacing = 1.75f;
        [SerializeField, FoldoutGroup("Formation")] private int maxUnitsPerRow = 8;
        [SerializeField, FoldoutGroup("Modifier")] private List<Modifier> movementModifiers = new List<Modifier>();

        private NavMeshAgent navMeshAgent;
        private bool hasMoveTarget;
        private Vector3 currentMoveTarget;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            ApplyAgentSettings();
            ForceIdle();
        }

        private void OnEnable()
        {
            ApplyAgentSettings();
            ForceIdle();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }

            ApplyAgentSettings();
        }

        private void Update()
        {
            TickMovement();
        }

        public void Move(float deltaTime, Vector2 targetPosition)
        {
            SetMoveTarget(new Vector3(targetPosition.x, transform.position.y, targetPosition.y));
        }

        public void SetMoveTarget(Vector3 worldTarget)
        {
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }

            if (navMeshAgent == null)
            {
                return;
            }

            ApplyAgentSettings();
            currentMoveTarget = worldTarget;
            hasMoveTarget = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(worldTarget);
        }

        public void Stop()
        {
            ForceIdle();
        }

        private void TickMovement()
        {
            if (!hasMoveTarget || navMeshAgent == null)
            {
                return;
            }

            if (navMeshAgent.pathPending)
            {
                return;
            }

            if (!navMeshAgent.hasPath)
            {
                hasMoveTarget = false;
                return;
            }

            var remainingDistance = navMeshAgent.remainingDistance;
            if (!float.IsInfinity(remainingDistance)
                && !float.IsNaN(remainingDistance)
                && remainingDistance <= Mathf.Max(stoppingDistance, navMeshAgent.stoppingDistance))
            {
                Stop();
            }
        }

        private void ApplyAgentSettings()
        {
            if (navMeshAgent == null)
            {
                return;
            }

            navMeshAgent.speed = Mathf.Max(0f, baseSpeed);
            navMeshAgent.acceleration = Mathf.Max(0f, baseAcceleration);
            navMeshAgent.angularSpeed = Mathf.Max(0f, turnSpeed);
            navMeshAgent.stoppingDistance = Mathf.Max(0.01f, stoppingDistance);
        }

        private void ForceIdle()
        {
            hasMoveTarget = false;

            if (navMeshAgent == null)
            {
                return;
            }

            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
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
