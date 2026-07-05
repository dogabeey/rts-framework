using Game.Core;
using UnityEngine;

namespace Game.GridSystem
{
    [System.Serializable]
    public class VisualData : IVisualData
    {

        public string Name { get; set; }
        public Mesh MeshReference { get; set; }
        public Material MaterialReference { get; set; }
        public Sprite SpriteReference { get; set; }
    }
}