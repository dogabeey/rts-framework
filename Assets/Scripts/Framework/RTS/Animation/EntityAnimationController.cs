using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Game.EventManagement;

namespace Game.RTS
{
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(EntityController))]
    public class EntityAnimationController : MonoBehaviour
    {
        [System.Serializable]
        public class StateAnimationMapping
        {
            public GameEvent gameEvent;
            [SerializeReference]
            public AnimationState state;
            [ValueDropdown("@$root.GetAllBooleanParameters()")]
            public string booleanParameterName;
        }
        [System.Serializable]
        public class TriggerAnimationMapping
        {
            public GameEvent gameEvent;
            [ValueDropdown("@$root.GetAllTriggerParameters()")]
            public string triggerParameterName;
        }

        private AnimationState currentAnimationState ;
        private readonly List<(GameEvent gameEvent, System.Action<EventParam> callback)> eventListeners = new List<(GameEvent, System.Action<EventParam>)>();
        [SerializeField] 
        private Animator animator;
        [SerializeField] 
        private EntityController entityController;
        [SerializeField, SerializeReference] 
        private AnimationState defaultAnimationState;
        [SerializeField]
        private StateAnimationMapping[] stateAnimationMappings = new StateAnimationMapping[0];
        [SerializeField]
        private TriggerAnimationMapping[] triggerAnimationMappings = new TriggerAnimationMapping[0];

        private void OnEnable()
        {
            foreach (var mapping in triggerAnimationMappings)
            {
                if (mapping == null) continue;
                var callback = new System.Action<EventParam>(e =>
                {
                    if (IsOwnEvent(e) && animator != null)
                        animator.SetTrigger(mapping.triggerParameterName);
                });
                eventListeners.Add((mapping.gameEvent, callback));
                EventManager.StartListening(mapping.gameEvent, callback);
            }
            foreach (var mapping in stateAnimationMappings)
            {
                if (mapping == null) continue;
                var callback = new System.Action<EventParam>(e =>
                {
                    if (!IsOwnEvent(e)) return;
                    CurrentAnimationState = mapping.state;
                    SetAnimatorBoolean(mapping.booleanParameterName);
                });
                eventListeners.Add((mapping.gameEvent, callback));
                EventManager.StartListening(mapping.gameEvent, callback);
            }
        }
        private void OnDisable()
        {
            foreach (var listener in eventListeners)
                EventManager.StopListening(listener.gameEvent, listener.callback);
            eventListeners.Clear();
        }

        private bool IsOwnEvent(EventParam eventParam)
        {
            return eventParam != null && eventParam.TryGet("entityController", out EntityController source)
                && source == entityController;
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

        private string GetBooleanParameterNameForState(AnimationState state)
        {
            foreach (var mapping in stateAnimationMappings)
            {
                if (mapping != null && mapping.state == state)
                {
                    return mapping.booleanParameterName;
                }
            }
            return null;
        }
        private void UpdateAnimatorState()
        {
            SetAnimatorBoolean(GetBooleanParameterNameForState(currentAnimationState ?? defaultAnimationState));
        }

        private void SetAnimatorBoolean(string booleanParameterName)
        {
            if (animator == null || string.IsNullOrEmpty(booleanParameterName))
            {
                return;
            }

            foreach (var mapping in stateAnimationMappings)
                if (mapping != null && !string.IsNullOrEmpty(mapping.booleanParameterName)) animator.SetBool(mapping.booleanParameterName, mapping.booleanParameterName == booleanParameterName);
        }
        public ValueDropdownList<string> GetAllBooleanParameters()
        {
            ValueDropdownList<string> booleanList = new ValueDropdownList<string>();

            // Loop through all parameters exposed on the controller
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    booleanList.Add(parameter.name, parameter.name);
                }
            }

            return booleanList;
        }
        public ValueDropdownList<string> GetAllTriggerParameters()
        {
            ValueDropdownList<string> triggerList = new ValueDropdownList<string>();

            // Loop through all parameters exposed on the controller
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    triggerList.Add(parameter.name, parameter.name);
                }
            }

            return triggerList;
        }
    }
}
