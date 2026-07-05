using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entity
{
    public abstract class Unit : Entity, IAttackable, IDamageable, IMoveable
    {
        public virtual float MaxHealth => baseMaxHealth;
        public virtual ArmorType ArmorType => armorType;
        public virtual float HitBox => hitBox;
        public virtual float Damage => baseDamage;
        public virtual DamageType AttackType => attackType;
        public virtual float Range => baseRange;
        public virtual float AttackCooldown => attackCooldown;
        public virtual float Speed => speed;
        public virtual float Acceleration => acceleration;
        public virtual float Deceleration => deceleration;
        public virtual bool IsFlyer => isFlyer;

        [FoldoutGroup("Health")]
        public float baseMaxHealth;
        [FoldoutGroup("Health")]
        public ArmorType armorType;
        [FoldoutGroup("Health")]
        public float hitBox;
        [FoldoutGroup("Attack")]
        public float baseDamage;
        [FoldoutGroup("Attack")]
        public DamageType attackType;
        [FoldoutGroup("Attack")]
        public float baseRange;
        [FoldoutGroup("Attack")]
        public float attackCooldown;
        [FoldoutGroup("Movement")]
        public float speed;
        [FoldoutGroup("Movement")]
        public float acceleration;
        [FoldoutGroup("Movement")]
        public float deceleration;
        [FoldoutGroup("Movement")]
        public bool isFlyer;


        public abstract void Attack(IDamageable target);
        public abstract bool CanAttack(IDamageable target);

        public void AttackMove(float deltaTime, Vector2 targetPosition)
        {
            throw new System.NotImplementedException();
        }

        public void Move(float deltaTime, Vector2 targetPosition)
        {
            throw new System.NotImplementedException();
        }

        public void TakeDamage(float amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
