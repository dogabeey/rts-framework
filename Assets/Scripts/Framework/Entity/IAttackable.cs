namespace Game.Entity
{
    public interface IAttackable
    {
        float BaseDamage { get; }
        DamageType AttackType { get; }
        float BaseRange { get; }
        float AttackCooldown { get; }

        void Attack(IDamageable target);
        bool CanAttack(IDamageable target);
    }
}
