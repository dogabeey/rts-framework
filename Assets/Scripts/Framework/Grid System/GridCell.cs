using Sirenix.OdinInspector;
using UnityEngine;
using Game.Core;    
using System;
using System.Collections.Generic;

namespace Game.GridSystem
{
    [Serializable]
    public abstract class GridCell : DynamicVisualObject
    {
        public abstract string CellTypeName { get; }
        public abstract Color EditorColor { get; }

        [ReadOnly] public List<GridElement> elements;

        public abstract void OnCurrentElementSet(GridElement newElement, GridElement oldElement); 
    }
}
