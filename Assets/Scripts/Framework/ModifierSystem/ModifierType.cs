using UnityEngine;

namespace Game.ModifierSystem
{
    [System.Serializable]
     public abstract class ModifierType
    {
        public enum ModifierCategory
        {
            Neutral,
            Buff,
            Debuff
        }
        public abstract string ModifierName { get; }
        public abstract string ModifierDescription { get; }
        public abstract ModifierCategory Category { get; }
        public abstract Sprite ModifierIcon { get; }
    }

    public class ModifierType<T> : ModifierType where T : ModifierType<T>
    {
        public override string ModifierName => typeof(T).Name;
        public override string ModifierDescription => "Description for " + typeof(T).Name;
        public override ModifierCategory Category => ModifierCategory.Neutral;
        public override Sprite ModifierIcon => null; // You can assign a default icon here if needed

        public static Modifier AddModifier(float value)
        {
            ModifierType modifierType = System.Activator.CreateInstance<T>();
            Modifier newModifier = new Modifier(modifierType, value);
            return newModifier;
        }
    }

    public class DummyModifier : ModifierType<DummyModifier>
    {
        public override string ModifierDescription => "This is a dummy modifier for testing purposes.";
        public override ModifierCategory Category => ModifierCategory.Neutral;
        public override Sprite ModifierIcon => null; // Assign an icon for dummy modifier
    }
}
