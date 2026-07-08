namespace Game.Entity
{
    [System.Serializable]
    public abstract class AttackStrategy
    {

        public abstract bool CanAttack(DamageableComponent target);
        public abstract void Attack(DamageableComponent target);
    }

    // Example concrete attack strategies
    [System.Serializable]
    public class MeleeAttackStrategy : AttackStrategy
    {
        public override bool CanAttack(DamageableComponent target)
        {
            // Implement logic to determine if the target is within melee range
            return true; // Placeholder
        }
        public override void Attack(DamageableComponent target)
        {
            // Implement melee attack logic
        }
    }

    // Example concrete attack strategies
    [System.Serializable]
    public class RangedAttackStrategy : AttackStrategy
    {
        public float Range;

        public override bool CanAttack(DamageableComponent target)
        {
            // Implement logic to determine if the target is within ranged range
            return true; // Placeholder
        }
        public override void Attack(DamageableComponent target)
        {
            // Implement ranged attack logic
        }
    }
}
