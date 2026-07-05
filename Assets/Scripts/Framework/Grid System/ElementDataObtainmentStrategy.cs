using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GridSystem
{
    public abstract class ElementDataObtainmentStrategy : ScriptableObject
    {
        public abstract Dictionary<Vector2Int, List<GridElement>> GenerateElements();
    }
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewAsepriteStrategy", menuName = "Grid/Element Data Strategy/Aseprite")]
    public class ElementDataObtainmentStrategy_Aseprite : ElementDataObtainmentStrategy
    {
        [SerializeField]
        private Texture2D bitmap;

        [SerializeReference, SerializeField]
        private List<GridElement> availableElementTypes;

        public override Dictionary<Vector2Int, List<GridElement>> GenerateElements()
        {
            Dictionary<Vector2Int, List<GridElement>> result = new();
            // TODO: Implement Aseprite parsing logic here. This is a placeholder implementation that simply logs the intention to parse the bitmap.    
            return result;
        }
    }
}