using UnityEngine;

namespace Game.Core
{
    public interface IVisualData
    {
        public abstract Mesh MeshReference { get; }
        public abstract Material MaterialReference { get; }
        public abstract Sprite SpriteReference { get; }

    }

    public static class VisualDataExtensions
    {
        public static void SetVisualData(this IVisualData visualData, ref Renderer renderer)
        {
            if(visualData.MaterialReference)
            {
                renderer.sharedMaterial = visualData.MaterialReference;
            }
            if (renderer is MeshRenderer meshRenderer)
            {
                if (visualData.MeshReference)
                {
                    meshRenderer.GetComponent<MeshFilter>().sharedMesh = visualData.MeshReference;
                }
            }
            else if (renderer is SpriteRenderer spriteRenderer)
            {
                if (visualData.SpriteReference)
                {
                    spriteRenderer.sprite = visualData.SpriteReference;
                }
            }
            else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
            {
                if (visualData.MeshReference)
                {
                    skinnedMeshRenderer.sharedMesh = visualData.MeshReference;
                }
            }
        }
    }
}