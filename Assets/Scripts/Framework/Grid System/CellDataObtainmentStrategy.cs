using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GridSystem
{
    public abstract class CellDataObtainmentStrategy : ScriptableObject
    {
        public abstract Dictionary<Vector2Int, GridCell> GenerateGrid();

    }
}