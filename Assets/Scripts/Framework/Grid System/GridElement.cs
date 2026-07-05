using System;
using UnityEngine;

namespace Game.GridSystem
{
    [Serializable]
    public abstract class GridElement : DynamicVisualObject
    {
        public Vector2Int Position { get; private set; }
        public abstract Color EditorColor { get; }
        public abstract int HeightSpacing { get; }

        public int indexInCell;
        public bool isLastElementOfIndex;

        public void InitializeElement(Vector2Int position)
        {
            Position = position;
        }
        public abstract void OnElementPlaced(GridCellController  cell);
        public abstract void OnElementRemoved(GridCellController cell);
    }
}