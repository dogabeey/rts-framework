namespace Game.Entity
{
    public interface IAttackable
    {
        float Damage { get; }
        DamageType AttackType { get; }
        float Range { get; }
        float AttackCooldown { get; }
        bool CanAttack(IDamageable target);
    }
}
