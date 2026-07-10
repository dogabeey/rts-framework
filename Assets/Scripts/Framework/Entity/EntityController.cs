using System;
using UnityEngine;
using Game.Core;

namespace Game.Entity
{
    public enum EntityState
    {
        idle,
        moving,
        flee,
        dead,

    }

    [RequireComponent( typeof(AttackableComponent), typeof(DamageableComponent))]
    public abstract class EntityController : MonoBehaviour
    {
        public EntityState EntityState
        {
            get
            {
                return entityState;
            }
            set
            {
                if(entityState != value)
                    OnEntityStateChange(value);
                entityState = value;
            }
        }

        protected virtual void OnEntityStateChange(EntityState value)
        {
            
        }

        protected abstract void UpdateEntityState();

        public Entity referenceEntity;
        public EntityState entityState;
        public Renderer entityRenderer;
        public AttackableComponent attackableComponent;
        public DamageableComponent damageableComponent;

        protected virtual void Start()
        {
            InitReferences();
        }
        protected virtual void Update() 
        {
            UpdateEntityState();
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
    }
}
