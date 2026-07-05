using UnityEngine;

namespace Game.Entity
{
    public abstract class Entity
    {
        public abstract string Name { get; set; }
        public abstract Mesh Mesh { get; set; }
        public Sprite Icon { get; set; }
    }
}
