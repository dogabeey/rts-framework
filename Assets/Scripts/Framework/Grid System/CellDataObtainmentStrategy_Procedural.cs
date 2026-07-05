using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GridSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewProceduralStrategy", menuName = "Grid/Cell Data Strategy/Procedural")]
    public class CellDataObtainmentStrategy_Procedural : CellDataObtainmentStrategy
    {
        public override Dictionary<Vector2Int, GridCell> GenerateGrid()
        {
            throw new NotImplementedException();
        }
    }
} 