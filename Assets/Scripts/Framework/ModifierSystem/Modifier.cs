namespace Game.ModifierSystem
{

    public class Modifier
    {
        public ModifierType modifierType;
        public float value;

        public enum ModifierCalculation
        {
            PreAdditive,
            Multiplicative,
            PostAdditive
        }

        public Modifier(ModifierType modifierType, float value)
        {
            this.modifierType = modifierType;
            this.value = value;
        }
    }
}
