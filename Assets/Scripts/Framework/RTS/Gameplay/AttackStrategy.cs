using UnityEngine;

namespace Game.RTS
{
    [System.Serializable]
    public abstract class AttackStrategy
    {
        [System.NonSerialized] private AttackableComponent attacker;
        protected AttackableComponent Attacker => attacker;

        public void Initialize(AttackableComponent attackableComponent)
        {
            attacker = attackableComponent;
        }

        public abstract bool CanAttack(DamageableComponent target);
        public abstract void Attack(DamageableComponent target);

        protected bool IsValidTarget(DamageableComponent target)
        {
            return attacker != null && target != null && !target.IsDead;
        }

        protected float HorizontalDistanceSquared(DamageableComponent target)
        {
            var offset = target.transform.position - attacker.transform.position;
            offset.y = 0f;
            return offset.sqrMagnitude;
        }

        protected void ApplyDamage(DamageableComponent target)
        {
            attacker?.ApplyDamage(target);
        }
    }

    // Default melee attack strategy.
    [System.Serializable]
    public class MeleeAttackStrategy : AttackStrategy
    {
        public override bool CanAttack(DamageableComponent target)
        {
            if (!IsValidTarget(target))
            {
                return false;
            }

            // A melee unit must reach the target's hitbox. AttackableComponent.range
            // intentionally remains an acquisition/scanning range, not melee reach.
            var meleeRange = Mathf.Max(0f, target.HitBox);
            return HorizontalDistanceSquared(target) <= meleeRange * meleeRange;
        }
        public override void Attack(DamageableComponent target)
        {
            ApplyDamage(target);
        }
    }

    // Default ranged attack strategy.
    [System.Serializable]
    public class RangedAttackStrategy : AttackStrategy
    {
        public float Range;

        public override bool CanAttack(DamageableComponent target)
        {
            if (!IsValidTarget(target))
            {
                return false;
            }

            var attackRange = Mathf.Max(0f, Range) + Mathf.Max(0f, target.HitBox);
            return HorizontalDistanceSquared(target) <= attackRange * attackRange;
        }
        public override void Attack(DamageableComponent target)
        {
            ApplyDamage(target);
        }
    }
}
