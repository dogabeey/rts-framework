using Game.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GridSystem
{
    /// <summary>
    /// Base class for ScriptableObjects that support dynamic visual data selection based on conditions.
    /// Evaluates multiple condition-to-visual mappings and applies the highest priority matching visual data.
    /// </summary>
    public abstract class DynamicVisualObject
    {
        /// <summary>
        /// Gets the integer value used for condition evaluation.
        /// </summary>
        public abstract int IntegerReference { get; }
        
        /// <summary>
        /// Gets the float value used for condition evaluation.
        /// </summary>
        public abstract float FloatReference { get; }
        
        /// <summary>
        /// Gets the GameObject reference used for condition evaluation.
        /// </summary>
        public abstract GameObject ObjectReference { get; }
        
        /// <summary>
        /// Gets the string value used for condition evaluation.
        /// </summary>
        public abstract string StringReference { get; }
        
        /// <summary>
        /// Gets the boolean value used for condition evaluation.
        /// </summary>
        public abstract bool BoolReference { get; }

        /// <summary>
        /// List of condition-to-visual data mappings. Each mapping associates a condition with visual data.
        /// </summary>
        public abstract List<VisualDataMap> VisualDataMappings { get; }
        /// <summary>
        /// Fallback visual data mapping to use when no conditions evaluate to true. This should provide a default visual representation.
        /// </summary>
        public abstract VisualDataMap FallbackVisualDataMap { get; }

        /// <summary>
        /// Evaluates all visual data mappings and applies the visual data from the highest priority matching condition to the renderer.
        /// </summary>
        /// <param name="renderer">The renderer to apply the visual data to.</param>
        public void SetVisualData(ref Renderer renderer)
        {
            if (VisualDataMappings != null && VisualDataMappings.Count > 0)
            {
                if (TryGetTrueConditionWithHighestPriority(VisualDataMappings, out VisualDataMap visualDataMap))
                {
                    Debug.Log($"Applying visual data from {visualDataMap.visualData.Name} to {this.GetType().Name} based on the highest priority condition.");
                    visualDataMap.visualData.SetVisualData(ref renderer);
                }
                else
                {
                    Debug.Log($"No conditions evaluated to true for {this.GetType().Name}. Applying fallback visual data.");
                    FallbackVisualDataMap.visualData.SetVisualData(ref renderer);
                }
            }
        }

        /// <summary>
        /// Evaluates all visual data mappings and returns the mapping with the highest priority whose condition evaluates to true.
        /// </summary>
        /// <param name="visualDataMappings">The list of visual data mappings to evaluate.</param>
        /// <param name="visualDataMap">The output mapping with the highest priority that has a true condition, or null if none match.</param>
        public bool TryGetTrueConditionWithHighestPriority(List<VisualDataMap> visualDataMappings, out VisualDataMap visualDataMap)
        {
            visualDataMap = null;
            int highestPriority = int.MinValue;

            foreach (var mapping in visualDataMappings)
            {
                int integerValue = 0;
                float floatValue = 0f;
                GameObject objectValue = null;
                string stringValue = null;
                bool boolValue = false;
                ExtractValues(out integerValue, out floatValue, out objectValue, out stringValue, out boolValue);
                if (mapping.condition.IsTrue(integerValue, floatValue, objectValue, stringValue, boolValue))
                {
                    if (mapping.condition.priority > highestPriority)
                    {
                        highestPriority = mapping.condition.priority;
                        visualDataMap = mapping;
                    }
                }
            }
            var visualDataList = string.Join(",", visualDataMappings.Select(vdm => vdm.visualData));
            if(visualDataMap != null)
            {
                Debug.Log($"Highest priority condition with the priority of {highestPriority} is {visualDataMap.visualData.MeshReference} " +
                    $"| {visualDataMap.visualData.MaterialReference}. Chosen among {visualDataList}");
                return true;
            }
            return false;
        }

        private void ExtractValues(out int integerValue, out float floatValue, out GameObject objectValue, out string stringValue, out bool boolValue)
        {
            try
            {
                integerValue = IntegerReference;
            }
            catch (System.NotImplementedException)
            {
                integerValue = int.MaxValue;
            }
            try
            {
                floatValue = FloatReference;
            }
            catch (System.NotImplementedException)
            {
                floatValue = float.MaxValue;
            }
            try
            {
                objectValue = ObjectReference;
            }
            catch (System.NotImplementedException)
            {
                objectValue = null;
            }
            try
            {
                stringValue = StringReference;
            }
            catch (System.NotImplementedException)
            {
                stringValue = null;
            }
            try
            {
                boolValue = BoolReference;
            }
            catch (System.NotImplementedException)
            {
                boolValue = false;
            }
        }
    }
}