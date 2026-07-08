using UnityEngine;

namespace Game.ModifierSystem
{

    public class Modifier
    {
        [SerializeReference]
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
        public static Modifier AddModifier<T>(float value) where T : ModifierType
        {
            ModifierType modifierType = System.Activator.CreateInstance<T>();
            Modifier newModifier = new Modifier(modifierType, value);
            return newModifier;
        }
    }
}
