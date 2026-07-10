using System;
using UnityEngine;
using Game.Core;

namespace Game.Entity
{
    // States to add: Idle, Sleep, Guard, Patrol, Attack, Flee, AttackMove
    public abstract class EntityMissionState
    {
        public abstract string StateName { get; }
        public abstract void OnEnterState(EntityController entityController);
        public abstract void OnExitState(EntityController entityController);
        public abstract void OnStateUpdate(EntityController entityController);
    }

    public class IdleState : EntityMissionState
    {
        public override string StateName => "Idle";

        public override void OnEnterState(EntityController entityController)
        {
            // Logic for entering idle state
        }

        public override void OnExitState(EntityController entityController)
        {
            // Logic for exiting idle state
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            // Logic for updating idle state
        }
    }
    public class SleepState : EntityMissionState
    {
        public override string StateName => "Sleep";

        public override void OnEnterState(EntityController entityController)
        {
            // Logic for entering sleep state
        }

        public override void OnExitState(EntityController entityController)
        {
            // Logic for exiting sleep state
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            // Logic for updating sleep state
        }
    }
    public class GuardState : EntityMissionState
    {
        public override string StateName => "Guard";

        public override void OnEnterState(EntityController entityController)
        {
            // Logic for entering guard state
        }

        public override void OnExitState(EntityController entityController)
        {
            // Logic for exiting guard state
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            // Logic for updating guard state
        }
    }
    public class PatrolState : EntityMissionState
    {
        public override string StateName => "Patrol";

        public override void OnEnterState(EntityController entityController)
        {
            // Logic for entering patrol state
        }

        public override void OnExitState(EntityController entityController)
        {
            // Logic for exiting patrol state
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            // Logic for updating patrol state
        }
    }
    public class AttackState : EntityMissionState
    {
        public override string StateName => "Attack";

        public override void OnEnterState(EntityController entityController)
        {
            // Logic for entering attack state
        }

        public override void OnExitState(EntityController entityController)
        {
            // Logic for exiting attack state
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            // Logic for updating attack state
        }
    }
    public class FleeState : EntityMissionState
    {
        public override string StateName => "Flee";

        public override void OnEnterState(EntityController entityController)
        {
            // Logic for entering flee state
        }

        public override void OnExitState(EntityController entityController)
        {
            // Logic for exiting flee state
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            // Logic for updating flee state
        }
    }
    public class AttackMoveState : EntityMissionState
    {
        public override string StateName => "AttackMove";

        public override void OnEnterState(EntityController entityController)
        {
            // Logic for entering attack move state
        }

        public override void OnExitState(EntityController entityController)
        {
            // Logic for exiting attack move state
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            // Logic for updating attack move state
        }
    }
}
