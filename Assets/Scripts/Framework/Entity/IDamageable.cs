using Sirenix.OdinInspector;

namespace Game.Entity
{
    public interface IDamageable
    {
        public float MaxHealth { get;}
        public ArmorType ArmorType { get; }
        public float HitBox { get; }

        void TakeDamage(float amount);
    }
}
