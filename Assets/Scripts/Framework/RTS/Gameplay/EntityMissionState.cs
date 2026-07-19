using System;
using UnityEngine;
using Game.Core;

namespace Game.RTS
{
    public enum EntityMissionType
    {
        Idle,
        Sleep,
        Guard,
        Patrol,
        Attack,
        Flee,
        AttackMove
    }

    public abstract class EntityMissionState
    {
        public abstract EntityMissionType MissionType { get; }
        public virtual string StateName => MissionType.ToString();

        public virtual void OnEnterState(EntityController entityController) { }
        public virtual void OnExitState(EntityController entityController) { }
        public virtual void OnStateUpdate(EntityController entityController) { }

        protected static MovementController GetMovementController(EntityController entityController)
        {
            if (entityController is UnitController unitController && unitController.movementController != null)
            {
                return unitController.movementController;
            }

            return entityController.GetComponent<MovementController>();
        }
    }

    public static class EntityMissionStateFactory
    {
        private static readonly IdleState Idle = new IdleState();
        private static readonly SleepState Sleep = new SleepState();
        private static readonly GuardState Guard = new GuardState();
        private static readonly PatrolState Patrol = new PatrolState();
        private static readonly AttackState Attack = new AttackState();
        private static readonly FleeState Flee = new FleeState();
        private static readonly AttackMoveState AttackMove = new AttackMoveState();

        public static EntityMissionState GetState(EntityMissionType missionType)
        {
            switch (missionType)
            {
                case EntityMissionType.Sleep:
                    return Sleep;
                case EntityMissionType.Guard:
                    return Guard;
                case EntityMissionType.Patrol:
                    return Patrol;
                case EntityMissionType.Attack:
                    return Attack;
                case EntityMissionType.Flee:
                    return Flee;
                case EntityMissionType.AttackMove:
                    return AttackMove;
                case EntityMissionType.Idle:
                default:
                    return Idle;
            }
        }
    }

    public class IdleState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Idle;

        public override void OnEnterState(EntityController entityController)
        {
            var movementController = GetMovementController(entityController);
            if (movementController != null)
            {
                movementController.Stop();
            }
        }
    }
    public class SleepState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Sleep;

        public override void OnEnterState(EntityController entityController)
        {
            var movementController = GetMovementController(entityController);
            if (movementController != null)
            {
                movementController.Stop();
            }
        }
    }
    public class GuardState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Guard;

        public override void OnEnterState(EntityController entityController)
        {
            var movementController = GetMovementController(entityController);
            if (movementController != null)
            {
                movementController.Stop();
            }
        }
    }
    public class PatrolState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Patrol;
    }
    public class AttackState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Attack;
    }
    public class FleeState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Flee;
    }
    public class AttackMoveState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.AttackMove;
    }
}
