namespace Game.Entity
{

    public interface IDamageable
    {
        public float Health { get; set; }
        public ArmorType ArmorType { get; set; }
        public float HitBox { get; set; }

        void TakeDamage(float amount);
    }
}
