using System;
using UnityEngine;
using Game.Core;

namespace Game.Entity
{
    [RequireComponent( typeof(AttackableComponent), typeof(DamageableComponent))]
    public abstract class EntityController : MonoBehaviour
    {
        public Entity referenceEntity;

        public Renderer entityRenderer;
        public AttackableComponent attackableComponent;
        public DamageableComponent damageableComponent;

        protected virtual void Start()
        {
            InitReferences();
        }
        protected virtual void InitReferences()
        {
            attackableComponent = GetComponent<AttackableComponent>();
            damageableComponent = GetComponent<DamageableComponent>();
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
