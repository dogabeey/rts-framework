using UnityEngine;
using Game.Core;

namespace Game.Entity
{
    public abstract class Entity : ScriptableObject, IVisualData
    {
        public abstract string Name { get; set; }
        public abstract Mesh Mesh { get; set; }
        public abstract Sprite Icon { get; set; }
        public Mesh MeshReference => meshReference;
        public Material MaterialReference => materialReference;
        public Sprite SpriteReference => spriteReference;

        public Mesh meshReference;
        public Material materialReference;
        public Sprite spriteReference;
    }
}
