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
        public abstract Sprite modifierIcon { get; }
    }
}
