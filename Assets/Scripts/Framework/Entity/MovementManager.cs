using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    [DisallowMultipleComponent]
    public class MovementManager : MonoBehaviour
    {
        [SerializeField] private float formationSpacing = 1.75f;
        [SerializeField] private int maxUnitsPerRow = 8;

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

            // Higher-priority units are assigned to front slots, lower-priority units to back slots.
            movers.Sort(CompareByFormationPriority);
            for (var i = 0; i < movers.Count; i++)
            {
                movers[i].movementController.SetMoveTarget(targetPosition + slots[i]);
            }
        }

        private static int CompareByFormationPriority(UnitController left, UnitController right)
        {
            var leftPriority = left.movementController != null && left.movementController.UseFormationPriority
                ? left.movementController.FormationPriority
                : 0;
            var rightPriority = right.movementController != null && right.movementController.UseFormationPriority
                ? right.movementController.FormationPriority
                : 0;

            return rightPriority.CompareTo(leftPriority);
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