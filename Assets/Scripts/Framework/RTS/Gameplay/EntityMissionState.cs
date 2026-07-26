using System;
using UnityEngine;
using Game.Core;

namespace Game.RTS
{
    public enum EntityMissionType
    {
        Idle, // Only attacks when an enemy is in range. Does not move unless commanded.
        Sleep, // Does not attack or move unless commanded. Can be used to disable AI temporarily.
        AreaGuard, //Attacks enemies in range and chases them until they leave guard radious (x2 of the attack range). Does not move unless commanded.
        Patrol, // Moves between waypoints and attacks enemies in range. Does not chase enemies outside of attack range.
        Attack, // Attacks a specific target until it is destroyed.
        Flee, // Avoids enemies when they are in range. Does not move unless commanded.
        AttackMove // Moves to a target location and attacks enemies in range. Does not chase enemies outside of attack range.
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
                case EntityMissionType.AreaGuard:
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
            entityController.ClearCurrentAttackTarget();
            var movementController = GetMovementController(entityController);
            if (movementController != null)
            {
                movementController.Stop();
            }
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            EntityMissionCombat.AttackIfEnemyIsInRange(entityController);
        }
    }
    public class SleepState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Sleep;

        public override void OnEnterState(EntityController entityController)
        {
            entityController.ClearCurrentAttackTarget();
            var movementController = GetMovementController(entityController);
            if (movementController != null)
            {
                movementController.Stop();
            }
        }
    }
    public class GuardState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.AreaGuard;

        public override void OnEnterState(EntityController entityController)
        {
            entityController.SetMissionAnchor(entityController.transform.position);
            entityController.ClearCurrentAttackTarget();
            var movementController = GetMovementController(entityController);
            if (movementController != null)
            {
                movementController.Stop();
            }
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            var attackable = entityController.attackableComponent;
            if (attackable == null || attackable.Range <= 0f)
            {
                return;
            }

            var target = entityController.CurrentAttackTarget;
            var guardRadius = attackable.Range * 2f;
            if (!entityController.IsValidEnemyTarget(target)
                || EntityMissionCombat.HorizontalDistanceSquared(entityController.MissionAnchor, target.transform.position) > guardRadius * guardRadius)
            {
                target = entityController.FindClosestEnemy(attackable.Range);
                entityController.SetCurrentAttackTarget(target);
            }

            if (target == null)
            {
                entityController.StopMovement();
                return;
            }

            if (entityController.IsTargetInAttackRange(target))
            {
                entityController.StopMovement();
                entityController.TryAttack(target);
            }
            else
            {
                entityController.ChaseTarget(target);
            }
        }
    }
    public class PatrolState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Patrol;

        public override void OnEnterState(EntityController entityController)
        {
            entityController.ClearCurrentAttackTarget();
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            // Patrols do not interrupt their waypoint route to chase.
            EntityMissionCombat.AttackIfEnemyIsInRange(entityController);
        }
    }
    public class AttackState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Attack;

        public override void OnStateUpdate(EntityController entityController)
        {
            var target = entityController.CurrentAttackTarget;
            if (!entityController.IsValidEnemyTarget(target))
            {
                entityController.ClearCurrentAttackTarget();
                entityController.SetMissionState(EntityMissionType.Idle);
                return;
            }

            if (entityController.IsTargetInAttackRange(target))
            {
                entityController.StopMovement();
                entityController.TryAttack(target);
            }
            else
            {
                entityController.ChaseTarget(target);
            }
        }
    }
    public class FleeState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.Flee;

        public override void OnEnterState(EntityController entityController)
        {
            entityController.ClearCurrentAttackTarget();
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            var attackable = entityController.attackableComponent;
            if (attackable == null || attackable.Range <= 0f)
            {
                return;
            }

            var threat = entityController.FindClosestEnemy(attackable.Range);
            if (threat == null)
            {
                return;
            }

            var direction = entityController.transform.position - threat.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = entityController.transform.forward;
            }

            entityController.MoveTo(entityController.transform.position + direction.normalized * attackable.Range);
        }
    }
    public class AttackMoveState : EntityMissionState
    {
        public override EntityMissionType MissionType => EntityMissionType.AttackMove;

        public override void OnEnterState(EntityController entityController)
        {
            entityController.ClearCurrentAttackTarget();
        }

        public override void OnStateUpdate(EntityController entityController)
        {
            // Attack-move retains its destination and only fires at enemies it passes.
            EntityMissionCombat.AttackIfEnemyIsInRange(entityController);
        }
    }

    internal static class EntityMissionCombat
    {
        public static void AttackIfEnemyIsInRange(EntityController entityController)
        {
            var attackable = entityController.attackableComponent;
            if (attackable == null || attackable.Range <= 0f)
            {
                return;
            }

            var target = entityController.FindClosestEnemy(attackable.Range);
            if (target != null)
            {
                entityController.TryAttack(target);
            }
        }

        public static float HorizontalDistanceSquared(Vector3 first, Vector3 second)
        {
            var offset = second - first;
            offset.y = 0f;
            return offset.sqrMagnitude;
        }
    }
}
