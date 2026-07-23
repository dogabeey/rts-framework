using UnityEngine;
using System.Reflection;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Game.RTS
{
    public enum TargetType
    {
        None,
        Position,
        Entity
    }

    [System.Serializable]
    public abstract class Order
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract Sprite Icon { get; }
        // Cursor when that order is selected
        public abstract Texture2D CursorTexture { get; }

        public abstract TargetType TargetType { get; }

        public abstract void ExecuteOrder(IEntityController entityController, Vector3 targetPosition, IEntityController targetEntityController);
        
        public static ValueDropdownList<string> GetAllInputActionFieldNamesInRTSInputActionAsset()
        {
            ValueDropdownList<string> inputActionFieldNames = new ValueDropdownList<string>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            foreach (FieldInfo field in typeof(RTS_InputActions).GetFields(bindingFlags))
            {
                if (field.FieldType != typeof(InputAction))
                {
                    continue;
                }

                if (!field.Name.StartsWith("m_RTS_"))
                {
                    continue;
                }

                inputActionFieldNames.Add(field.Name.Replace("m_RTS_", "").Replace("_", " "), field.Name);
            }

            return inputActionFieldNames;
        }
    }

    public abstract class EntityTargetedOrder : Order
    {
        public override TargetType TargetType => TargetType.Entity;
        public abstract List<UnitType> TargetableUnitTypes { get; }
        public abstract List<BuildingType> TargetableBuildingTypes { get; }
    }
    public abstract class PositionTargetedOrder : Order
    {
        public override TargetType TargetType => TargetType.Position;
    }

    public class MoveOrder : PositionTargetedOrder
    {
        [SerializeField] private string name = "Move";
        [SerializeField] private string description = "Move to the selected position.";
        [SerializeField] private Sprite icon;
        [SerializeField] private Texture2D cursorTexture;

        public override string Name => name;
        public override string Description => description;
        public override Sprite Icon => icon;
        public override Texture2D CursorTexture => cursorTexture;

        public override void ExecuteOrder(IEntityController entityController, Vector3 targetPosition, IEntityController targetEntityController)
        {
            entityController.MoveTo(targetPosition);
        }
    }
}
