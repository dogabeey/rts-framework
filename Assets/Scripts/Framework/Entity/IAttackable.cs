namespace Game.Entity
{
    public interface IAttackable
    {
        float Damage { get; }
        DamageType AttackType { get; }
        float Range { get; }
        float AttackCooldown { get; }
        AttackStrategy AttackStrategy { get; }

        public void Attack(IDamageable target);
        public bool CanAttack(IDamageable target);
    }
}
