using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.GridSystem
{
    public abstract class Grid2D : MonoBehaviour
    {
        [Header("Grid 2D")]
        public GridElementController gridElementPrefab;
        [SerializeField] protected GridCellController cellControllerPrefab;
        [SerializeField, SerializeReference] protected CellDataObtainmentStrategy cellDataObtainmentStrategy;
        [SerializeField, SerializeReference] protected ElementDataObtainmentStrategy elementDataObtainmentStrategy;
        [SerializeField] protected Vector2 cellSpacing;
        [SerializeField] protected Dictionary<Vector2Int, GridCell> gridCells;
        [SerializeField] protected Transform parent;

        protected Dictionary<Vector2Int, Transform> generatedTiles = new();
        protected List<GridElement> generatedElements = new();

        protected virtual IEnumerator Start()
        {
            yield return new WaitUntil(() => GfxManagement.GfxManager.Instance != null && GfxManagement.GfxManager.Instance.assetsLoaded);
            ObtainCellData();
            InstantiateCells();
            ObtainElementData();
            InstantiateElements();
            PreInit();
            Init();
            PostInit();
            yield return null;
        }

        protected void ObtainCellData()
        {
            if (cellDataObtainmentStrategy == null)
            {
                Debug.LogError("No CellDataObtainmentStrategy assigned to Grid3D.");
                return;
            }
            gridCells = cellDataObtainmentStrategy.GenerateGrid();
        }

        protected void ObtainElementData()
        {
            if (elementDataObtainmentStrategy == null)
            {
                Debug.LogError("No ElementDataObtainmentStrategy assigned to Grid3D.");
                return;
            }

            Dictionary<Vector2Int, List<GridElement>> elementData = elementDataObtainmentStrategy.GenerateElements();
            foreach (var kvp in elementData)
            {
                Vector2Int cellPos = new Vector2Int(kvp.Key.x, kvp.Key.y);
                if (!gridCells.ContainsKey(cellPos))
                {
                    Debug.LogWarning($"Element data contains position {cellPos} which is outside the grid bounds.");
                    continue;
                }
                gridCells[cellPos].elements ??= new List<GridElement>();
                gridCells[cellPos].elements.AddRange(kvp.Value);
                // Set index for each element in the cell
                for (int i = 0; i < gridCells[cellPos].elements.Count; i++)
                {
                    gridCells[cellPos].elements[i].indexInCell = i;
                    if(i == gridCells[cellPos].elements.Count - 1)
                    {
                        gridCells[cellPos].elements[i].isLastElementOfIndex = true;
                    }
                }
            }
        }
        protected virtual void Init() 
        {

        }

        protected abstract void InstantiateCells();
        protected abstract void InstantiateElements();

        /// <summary>
        /// Called on Start() before any base class initialization.
        /// </summary>
        public abstract void PreInit();
        /// <summary>
        /// Called on Start() after all base class initializations.
        /// </summary>
        public abstract void PostInit();

        [System.Flags]
        public enum Axis
        {
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2,
            All = X | Y | Z
        }

    }
} 