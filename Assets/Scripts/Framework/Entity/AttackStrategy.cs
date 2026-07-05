namespace Game.Entity
{
    [System.Serializable]
    public abstract class AttackStrategy
    {
        public abstract bool CanAttack(IDamageable target);
        public abstract void Attack(IDamageable target);
    }

    // Example concrete attack strategies
    [System.Serializable]
    public class MeleeAttackStrategy : AttackStrategy
    {
        public override bool CanAttack(IDamageable target)
        {
            // Implement logic to determine if the target is within melee range
            return true; // Placeholder
        }
        public override void Attack(IDamageable target)
        {
            // Implement melee attack logic
        }
    }

    // Example concrete attack strategies
    [System.Serializable]
    public class RangedAttackStrategy : AttackStrategy
    {
        public override bool CanAttack(IDamageable target)
        {
            // Implement logic to determine if the target is within ranged range
            return true; // Placeholder
        }
        public override void Attack(IDamageable target)
        {
            // Implement ranged attack logic
        }
    }
}
