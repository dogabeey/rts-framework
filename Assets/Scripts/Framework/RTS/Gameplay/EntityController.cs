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
                    return target.factionID != 0 && target.allianceID != 0 && allianceID != target.allianceID;

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
            if(this is UnitController unitController)
            {
                unitController.MoveTo(targetPosition);
            }
        }
    }
}
