using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    [DisallowMultipleComponent]
    public class MovementManager : MonoBehaviour
    {
        public void CommandMove(IReadOnlyList<UnitController> units, Vector3 targetPosition)
        {
            if (units == null || units.Count == 0)
            {
                return;
            }

            var movers = new List<UnitController>(units.Count);
            foreach (var unit in units)
            {
                if (unit == null || unit.movementController == null)
                {
                    continue;
                }

                movers.Add(unit);
            }

            if (movers.Count == 0)
            {
                return;
            }

            var formationSpacing = ResolveFormationSpacing(movers);
            var maxUnitsPerRow = ResolveMaxUnitsPerRow(movers);

            var center = CalculateCenter(movers);
            var forward = targetPosition - center;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = Vector3.forward;
            }
            else
            {
                forward.Normalize();
            }

            var right = Vector3.Cross(Vector3.up, forward).normalized;
            var rowSize = Mathf.Clamp(maxUnitsPerRow, 1, movers.Count);
            var slots = BuildSlots(movers.Count, rowSize, Mathf.Max(0.1f, formationSpacing), right, forward);

            // Higher-priority units are assigned to front slots; same-type units are kept adjacent.
            movers.Sort(CompareByPriorityThenType);
            for (var i = 0; i < movers.Count; i++)
            {
                movers[i].movementController.SetMoveTarget(targetPosition + slots[i]);
            }
        }

        private static int CompareByPriorityThenType(UnitController left, UnitController right)
        {
            var leftPriority = left.movementController != null && left.movementController.UseFormationPriority
                ? left.movementController.FormationPriority
                : 0;
            var rightPriority = right.movementController != null && right.movementController.UseFormationPriority
                ? right.movementController.FormationPriority
                : 0;

            var priorityComparison = rightPriority.CompareTo(leftPriority);
            if (priorityComparison != 0)
            {
                return priorityComparison;
            }

            var leftType = left.Unit != null ? left.Unit.GetType().Name : string.Empty;
            var rightType = right.Unit != null ? right.Unit.GetType().Name : string.Empty;
            var typeComparison = string.CompareOrdinal(leftType, rightType);
            if (typeComparison != 0)
            {
                return typeComparison;
            }

            return left.GetInstanceID().CompareTo(right.GetInstanceID());
        }

        private static float ResolveFormationSpacing(List<UnitController> units)
        {
            var spacing = 0.1f;
            for (var i = 0; i < units.Count; i++)
            {
                var movementController = units[i].movementController;
                if (movementController == null)
                {
                    continue;
                }

                spacing = Mathf.Max(spacing, movementController.FormationSpacing);
            }

            return spacing;
        }

        private static int ResolveMaxUnitsPerRow(List<UnitController> units)
        {
            var row = 1;
            for (var i = 0; i < units.Count; i++)
            {
                var movementController = units[i].movementController;
                if (movementController == null)
                {
                    continue;
                }

                row = Mathf.Max(row, movementController.MaxUnitsPerRow);
            }

            return row;
        }

        private static Vector3 CalculateCenter(List<UnitController> units)
        {
            var sum = Vector3.zero;
            for (var i = 0; i < units.Count; i++)
            {
                sum += units[i].transform.position;
            }

            return sum / units.Count;
        }

        private static List<Vector3> BuildSlots(int count, int rowSize, float spacing, Vector3 right, Vector3 forward)
        {
            var slots = new List<Vector3>(count);

            var row = 0;
            while (slots.Count < count)
            {
                var unitsRemaining = count - slots.Count;
                var thisRowCount = Mathf.Min(rowSize, unitsRemaining);
                var rowWidth = (thisRowCount - 1) * spacing;
                var rowOffset = -forward * row * spacing;

                for (var i = 0; i < thisRowCount; i++)
                {
                    var xOffset = -rowWidth * 0.5f + i * spacing;
                    slots.Add(rowOffset + right * xOffset);
                }

                row++;
            }

            return slots;
        }
    }
}