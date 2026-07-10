using System;

namespace Game.Entity
{
    [Serializable]
    public abstract class AnimationState
    {
        public abstract string ParameterName { get; }
        public abstract void OnEnter(EntityAnimationController controller);
        public abstract void OnExit(EntityAnimationController controller);
        public abstract void OnUpdate(EntityAnimationController controller);
    }

    [Serializable]
    public class IdleAnimationState : AnimationState
    {
        public override string ParameterName => "Idle";

        public override void OnEnter(EntityAnimationController controller)
        {
            // Implement logic for entering idle state
        }

        public override void OnExit(EntityAnimationController controller)
        {
            // Implement logic for exiting idle state
        }

        public override void OnUpdate(EntityAnimationController controller)
        {
            // Implement logic for updating idle state
        }
    }
    [Serializable]
    public class MovingAnimationState : AnimationState
    {
        public override string ParameterName => "Moving";

        public override void OnEnter(EntityAnimationController controller)
        {
            // Implement logic for entering moving state
        }

        public override void OnExit(EntityAnimationController controller)
        {
            // Implement logic for exiting moving state
        }

        public override void OnUpdate(EntityAnimationController controller)
        {
            // Implement logic for updating moving state
        }
    }
    [Serializable]
    public class AttackingAnimationState : AnimationState
    {
        public override string ParameterName => "Attacking";

        public override void OnEnter(EntityAnimationController controller)
        {
            // Implement logic for entering attacking state
        }

        public override void OnExit(EntityAnimationController controller)
        {
            // Implement logic for exiting attacking state
        }

        public override void OnUpdate(EntityAnimationController controller)
        {
            // Implement logic for updating attacking state
        }
    }
    [Serializable]
    public class DeathAnimationState : AnimationState
    {
        public override string ParameterName => "Dying";

        public override void OnEnter(EntityAnimationController controller)
        {
            // Implement logic for entering dying state
        }

        public override void OnExit(EntityAnimationController controller)
        {
            // Implement logic for exiting dying state
        }

        public override void OnUpdate(EntityAnimationController controller)
        {
            // Implement logic for updating dying state
        }
    }
}
