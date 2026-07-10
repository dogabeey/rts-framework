using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.EventManagement;

namespace Game.Entity
{
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(EntityController))]
    public class EntityAnimationController : MonoBehaviour
    {
        [System.Serializable]
        public class StateAnimationMapping
        {
            [SerializeReference]
            public AnimationState state;
            public string booleanParameterName;
        }
        public class TriggerAnimationMapping
        {
            public GameEvent gameEvent;
            public string triggerParameterName;
        }

        private AnimationState currentAnimationState ;
        [SerializeField] 
        private Animator animator;
        [SerializeField] 
        private EntityController entityController;
        [SerializeField, SerializeReference] 
        private AnimationState defaultAnimationState;
        [SerializeField]
        private StateAnimationMapping[] stateAnimationMappings;
        [SerializeField]
        private TriggerAnimationMapping[] triggerAnimationMappings;

        private void OnEnable()
        {
            foreach (var mapping in triggerAnimationMappings)
            {
                EventManager.StartListening(mapping.gameEvent, (EventParam e) =>
                    {
                        if(e.paramDictionary != null 
                        && e.paramDictionary.ContainsKey("Entity")
                        && e.paramDictionary.TryGetValue("Entity", out object entityObj)
                        && entityObj is EntityController entityController
                        && entityController == this.entityController)
                        {
                            animator.SetTrigger(mapping.triggerParameterName);
                        }
                    }
                );
            }
        }
        private void OnDisable()
        {
            foreach (var mapping in triggerAnimationMappings)
            {
                EventManager.StopListening(mapping.gameEvent, (EventParam e) =>
                    {
                        if(e.paramDictionary != null 
                        && e.paramDictionary.ContainsKey("Entity")
                        && e.paramDictionary.TryGetValue("Entity", out object entityObj)
                        && entityObj is EntityController entityController
                        && entityController == this.entityController)
                        {
                            animator.SetTrigger(mapping.triggerParameterName);
                        }
                    }
                );
            }
        }


        public AnimationState CurrentAnimationState
        {
            get => currentAnimationState;
            set
            {
                if (currentAnimationState != value)
                {
                    currentAnimationState = value;
                    UpdateAnimatorState();

                    EventParam animationParam = new EventParam();
                    animationParam.Set(EventParam.Keys.GameObject, gameObject);
                    animationParam.Set("entityController", entityController);
                    animationParam.Set("animationState", currentAnimationState != null ? currentAnimationState.ParameterName : string.Empty);
                    EventManager.TriggerEvent(GameEvent.ENTITY_ANIMATION_STATE_CHANGED, animationParam);
                } 
            }
        }

        private void UpdateAnimatorState()
        {
            // Implement animator update logic here
        }
    }
}
