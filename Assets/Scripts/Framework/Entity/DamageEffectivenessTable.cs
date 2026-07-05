namespace Game.Entity
{
    public static class DamageEffectivenessTable
    {
        private static readonly float[,] Multipliers =
        {
            { 1.25f, 1.10f, 0.80f, 0.95f, 1.00f, 1.05f, 1.00f },
            { 0.80f, 0.90f, 1.20f, 1.00f, 0.95f, 1.00f, 1.10f },
            { 0.95f, 1.05f, 0.90f, 1.10f, 1.00f, 1.25f, 0.90f },
            { 1.00f, 0.80f, 1.10f, 0.90f, 1.10f, 0.75f, 1.25f },
            { 1.00f, 1.00f, 0.95f, 1.20f, 0.75f, 1.00f, 1.05f },
            { 0.85f, 0.90f, 1.25f, 1.10f, 1.00f, 1.05f, 1.00f }
        };

        public static float GetMultiplier(ArmorType armorType, DamageType damageType)
        {
            return Multipliers[(int)armorType, (int)damageType];
        }
    }
}
