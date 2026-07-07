using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entity
{
    [CreateAssetMenu(fileName = "New Building", menuName = "RTS Framework/Entity/New Building...")]
    public class Building : Entity, IDamageable, IAttackable
    {
        public override string Name { get; set; }
        public override Mesh Mesh { get; set; }
        public override Sprite Icon { get; set; }
        public virtual float MaxHealth => baseMaxHealth; // TODO: Implement modifiers for health (e.g., buffs, debuffs, etc.)
        public virtual ArmorType ArmorType => armorType;
        public virtual float HitBox => hitBox;
        public virtual float Damage => baseDamage; // TODO: Implement modifiers for damage (e.g., buffs, debuffs, etc.)
        public virtual AttackStrategy AttackStrategy => attackStrategy;
        public virtual DamageType AttackType => attackType;
        public virtual float Range
        {
            get
            {
                if (AttackStrategy is RangedAttackStrategy rangedAttackStrategy)
                {
                    return rangedAttackStrategy.Range;
                }
                else
                {
                    return 0; // Default range for melee units or units without a specific range
                }
            }
        }
        public virtual float AttackCooldown => attackCooldown; // TODO: Implement modifiers for attack cooldown (e.g., buffs, debuffs, etc.)
        public virtual float Speed => speed; // TODO: Implement modifiers for speed (e.g., buffs, debuffs, etc.)
        public virtual float Acceleration => acceleration;
        public virtual float Deceleration => deceleration;
        public virtual bool IsFlyer => isFlyer;

        [FoldoutGroup("General")]
        public string _name;
        [FoldoutGroup("General")]
        public Mesh mesh;
        [FoldoutGroup("General")]
        public Sprite icon;
        [FoldoutGroup("Health")]
        public float baseMaxHealth;
        [FoldoutGroup("Health")]
        public ArmorType armorType;
        [FoldoutGroup("Health")]
        public float hitBox;
        [FoldoutGroup("Attack")]
        public float baseDamage;
        [FoldoutGroup("Attack")]
        [SerializeReference] public AttackStrategy attackStrategy;
        [FoldoutGroup("Attack")]
        public DamageType attackType;
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


        public void Attack(IDamageable target) => AttackStrategy.Attack(target);
        public bool CanAttack(IDamageable target) => AttackStrategy.CanAttack(target);

        public void TakeDamage(float amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
