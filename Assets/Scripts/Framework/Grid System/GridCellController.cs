using Game.Core;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GridSystem
{
    public abstract class GridCellController : MonoBehaviour
    {
        [ReadOnly] public Grid2D parentGrid;
        [ReadOnly] public GridCell referenceCell;
        [ReadOnly] public List<GridElementController> currentElements;
        [ReadOnly] public Vector2Int coordinates;

        [SerializeField] private Renderer cellRenderer;

        public void Initialize(Grid2D parentGrid, GridCell cell, Vector2Int coords)
        {
            this.parentGrid = parentGrid;
            referenceCell = cell;
            coordinates = new Vector2Int(coords.x, coords.y);
            referenceCell.SetVisualData(ref cellRenderer);
            if (referenceCell.elements != null)
                SetCurrentElements(referenceCell.elements, coords);
        }

        public void SetCurrentElements(List<GridElement> elements, Vector2Int coords)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                GridElement element = elements[i];
                AddElement(element);
            }
        }

        public void AddElement(GridElement element)
        {
            GridElementController elementControllerPrefab = parentGrid.gridElementPrefab;
            GridElementController elementController = Instantiate(elementControllerPrefab, parentGrid.transform);
            currentElements.Add(elementController);
            elementController.Initialize(element, coordinates);
            element.OnElementPlaced(this);
        }
        public void RemoveElement(GridElementController elementController)
        {
            if (currentElements.Contains(elementController))
            {
                currentElements.Remove(elementController);
                elementController.referenceElement.OnElementRemoved(this);
            }
        }
    }
}
