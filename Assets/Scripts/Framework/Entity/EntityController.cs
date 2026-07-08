using System;
using UnityEngine;
using Game.Core;

namespace Game.Entity
{
    public abstract class EntityController : MonoBehaviour
    {
        [HideInInspector]
        public Entity referenceEntity;

        public Renderer entityRenderer;

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
