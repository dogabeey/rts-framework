using UnityEngine;
using Game.Core;
using Sirenix.OdinInspector;

namespace Game.RTS
{
    public abstract class Entity : ScriptableObject, IVisualData
    {
        public Mesh MeshReference => meshReference;
        public Material MaterialReference => materialReference;
        public Sprite SpriteReference => spriteReference;

        public Mesh meshReference;
        public Material materialReference;
        public Sprite spriteReference;

        [FoldoutGroup("General")]
        public string _name;
    }
}
