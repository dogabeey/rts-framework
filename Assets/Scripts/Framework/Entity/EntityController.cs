using UnityEngine;
using Game.Core;
using System;

namespace Game.Entity
{

    [RequireComponent( typeof(AttackableComponent), typeof(DamageableComponent))]
    public abstract class EntityController : MonoBehaviour
    {
        public EntityMissionState EntityState => entityState;

        public event Action<EntityMissionState, EntityMissionState> StateChanged;

        [SerializeField] protected EntityMissionType initialMissionState = EntityMissionType.Idle;

        public Entity referenceEntity;
        [SerializeField] protected EntityMissionState entityState;
        public Renderer entityRenderer;
        public AttackableComponent attackableComponent;
        public DamageableComponent damageableComponent;

        protected virtual void Start()
        {
            InitReferences();
            SetMissionState(initialMissionState, true);
        }
        protected virtual void Update() 
        {
            UpdateEntityState();
        }

        protected virtual void UpdateEntityState()
        {
            entityState?.OnStateUpdate(this);
            SetMissionState(EntityMissionType.Flee);
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
        }
    }
}
