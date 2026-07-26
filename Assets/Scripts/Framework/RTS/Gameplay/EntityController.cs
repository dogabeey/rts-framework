using UnityEngine;
using Game.Core;
using System;
using Game.EventManagement;
using Sirenix.OdinInspector;

namespace Game.RTS
{

    [RequireComponent( typeof(AttackableComponent), typeof(DamageableComponent))]
    public abstract class EntityController : MonoBehaviour, IEntityController
    {
        public GameObject GameObject => gameObject;

        public EntityMissionState EntityState => entityState;

        public event Action<EntityMissionState, EntityMissionState> StateChanged;

        [SerializeField] protected EntityMissionType initialMissionState = EntityMissionType.Idle;
        [SerializeField] protected EntityMissionState entityState;
        public Entity referenceEntity;
        public Renderer entityRenderer;
        public AttackableComponent attackableComponent;
        public DamageableComponent damageableComponent;
        public int factionID;
        public int allianceID;
        public DamageableComponent CurrentAttackTarget => currentAttackTarget;
        public Vector3 MissionAnchor => missionAnchor;

        [SerializeField, ReadOnly] private DamageableComponent currentAttackTarget;
        [SerializeField, ReadOnly] private Vector3 missionAnchor;

        /// <summary>
        /// Returns whether <paramref name="target"/> has the requested relationship to this entity.
        /// An ID of zero represents a neutral faction or alliance.
        /// </summary>
        public bool IsTargetInScope(EntityController target, TargetScope targetScope)
        {
            if (target == null)
            {
                return false;
            }

            switch (targetScope)
            {
                case TargetScope.Any:
                    return true;

                case TargetScope.SameFaction:
                    return factionID == target.factionID;

                case TargetScope.Allied:
                    return factionID != target.factionID && allianceID != 0 && allianceID == target.allianceID;

                case TargetScope.Enemy:
                    return factionID != 0 && target.factionID != 0
                        && factionID != target.factionID
                        && allianceID != 0 && target.allianceID != 0
                        && allianceID != target.allianceID;

                case TargetScope.Neutral:
                    return target.factionID == 0 || target.allianceID == 0;

                default:
                    return false;
            }
        }

        protected virtual void Start()
        {
            InitReferences();
            SetMissionState(initialMissionState, true);

            EventParam spawnParam = new EventParam();
            spawnParam.Set(EventParam.Keys.GameObject, gameObject);
            spawnParam.Set("entityController", this);
            EventManager.TriggerEvent(GameEvent.ENTITY_SPAWNED, spawnParam);
        }
        protected virtual void Update() 
        {
            UpdateEntityState();
        }

        protected virtual void UpdateEntityState()
        {
            if (damageableComponent != null && damageableComponent.IsDead)
            {
                return;
            }

            entityState?.OnStateUpdate(this);
        }

        protected virtual void OnDestroy()
        {
            EventParam destroyParam = new EventParam();
            destroyParam.Set(EventParam.Keys.GameObject, gameObject);
            destroyParam.Set("entityController", this);
            EventManager.TriggerEvent(GameEvent.ENTITY_DESTROYED, destroyParam);
        }

        protected virtual void InitReferences()
        {
            attackableComponent = GetComponent<AttackableComponent>();
            if(attackableComponent) attackableComponent.referenceEntity = this;
            damageableComponent = GetComponent<DamageableComponent>();
            if(damageableComponent) damageableComponent.referenceEntity = this;
            if (referenceEntity) // If referenceEntity is already assigned in the inspector, bind visuals immediately
            {
                OnSetReference();
            }
        }
        public void SetReference(Entity referenceEntity)
        {
            this.referenceEntity = referenceEntity;
            OnSetReference();
        }

        private void OnSetReference()
        {
            BindVisuals();
        }

        private void BindVisuals()
        {
            referenceEntity.SetVisualData(ref entityRenderer);
        }

        public bool SetMissionState(EntityMissionType missionType, bool force = false)
        {
            return ChangeState(EntityMissionStateFactory.GetState(missionType), force);
        }

        public bool ChangeState(EntityMissionState nextState, bool force = false)
        {
            if (nextState == null)
            {
                return false;
            }

            if (!force && entityState == nextState)
            {
                return false;
            }

            var previousState = entityState;
            previousState?.OnExitState(this);

            entityState = nextState;
            entityState.OnEnterState(this);

            OnEntityStateChange(previousState, entityState);
            StateChanged?.Invoke(previousState, entityState);
            return true;
        }

        protected virtual void OnEntityStateChange(EntityMissionState previousState, EntityMissionState nextState)
        {
            EventParam missionParam = new EventParam();
            missionParam.Set(EventParam.Keys.GameObject, gameObject);
            missionParam.Set("entityController", this);
            missionParam.Set("previousMissionState", previousState != null ? previousState.StateName : string.Empty);
            missionParam.Set("nextMissionState", nextState != null ? nextState.StateName : string.Empty);
            missionParam.Set("previousMissionType", previousState != null ? previousState.MissionType : EntityMissionType.Idle);
            missionParam.Set("nextMissionType", nextState != null ? nextState.MissionType : EntityMissionType.Idle);
            EventManager.TriggerEvent(GameEvent.ENTITY_MISSION_STATE_CHANGED, missionParam);
        }

        public void MoveTo(Vector3 targetPosition)
        {
            var movement = GetComponent<MovementController>();
            if (movement != null && (!movement.HasMoveTarget || (movement.CurrentMoveTarget - targetPosition).sqrMagnitude > 0.25f))
            {
                movement.SetMoveTarget(targetPosition);
            }
        }

        /// <summary>Orders this entity to pursue and attack one enemy until it dies or the order is replaced.</summary>
        public bool SetAttackTarget(EntityController target)
        {
            return SetAttackTarget(target != null ? target.damageableComponent : null);
        }

        public bool SetAttackTarget(DamageableComponent target)
        {
            if (!IsValidEnemyTarget(target))
            {
                return false;
            }

            currentAttackTarget = target;
            return SetMissionState(EntityMissionType.Attack, true);
        }

        public bool IsValidEnemyTarget(DamageableComponent target)
        {
            return target != null && !target.IsDead && target.referenceEntity != null
                && target.referenceEntity != this && IsTargetInScope(target.referenceEntity, TargetScope.Enemy);
        }

        public DamageableComponent FindClosestEnemy(float searchRange)
        {
            DamageableComponent closest = null;
            var bestDistance = float.MaxValue;
            foreach (var candidate in DamageableComponent.All)
            {
                if (!IsValidEnemyTarget(candidate))
                {
                    continue;
                }

                var offset = candidate.transform.position - transform.position;
                offset.y = 0f;
                var distance = offset.sqrMagnitude;
                var effectiveRange = Mathf.Max(0f, searchRange) + Mathf.Max(0f, candidate.HitBox);
                if (distance <= effectiveRange * effectiveRange && distance < bestDistance)
                {
                    closest = candidate;
                    bestDistance = distance;
                }
            }

            return closest;
        }

        public bool IsTargetInAttackRange(DamageableComponent target)
        {
            return attackableComponent != null && attackableComponent.CanAttack(target);
        }

        public bool TryAttack(DamageableComponent target)
        {
            return IsValidEnemyTarget(target) && attackableComponent != null && attackableComponent.TryAttack(target);
        }

        public void SetMissionAnchor(Vector3 anchor) => missionAnchor = anchor;
        public void SetCurrentAttackTarget(DamageableComponent target) => currentAttackTarget = target;
        public void ClearCurrentAttackTarget() => currentAttackTarget = null;

        public void ChaseTarget(DamageableComponent target)
        {
            var movement = GetComponent<MovementController>();
            if (movement == null || target == null)
            {
                return;
            }

            var targetPosition = target.transform.position;
            if (!movement.HasMoveTarget || (movement.CurrentMoveTarget - targetPosition).sqrMagnitude > 0.25f)
            {
                movement.SetMoveTarget(targetPosition);
            }
        }

        public void StopMovement()
        {
            GetComponent<MovementController>()?.Stop();
        }
    }
}
