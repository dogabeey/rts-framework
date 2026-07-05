using UnityEngine;

namespace Game.Entity
{
    public abstract class Entity : ScriptableObject
    {
        public abstract string Name { get; set; }
        public abstract Mesh Mesh { get; set; }
        public abstract Sprite Icon { get; set; }
    }
}
