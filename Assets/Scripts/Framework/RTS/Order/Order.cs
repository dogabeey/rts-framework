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
    public enum TargetScope
    {
        Any,
        SameFaction,
        Allied,
        Enemy,
        Neutral
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

        public abstract void ExecuteOrder(EntityController entityController, Vector3 targetPosition, EntityController targetEntityController);
        
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

    [System.Serializable]
    public abstract class EntityTargetedOrder : Order
    {
        public override TargetType TargetType => TargetType.Entity;
        public abstract List<UnitType> TargetableUnitTypes { get; }
        public abstract List<BuildingType> TargetableBuildingTypes { get; }
        public abstract List<TargetScope> TargetableScopes { get; }
    }
    [System.Serializable]
    public abstract class PositionTargetedOrder : Order
    {
        public override TargetType TargetType => TargetType.Position;
    }
    public class TestNonTargetedOrder : Order
    {
        public override string Name => "Test Non Targeted Order";
        public override string Description => "This is a test order that does not require a target.";
        public override Sprite Icon => null;
        public override Texture2D CursorTexture => null;
        public override TargetType TargetType => TargetType.None;

        public override void ExecuteOrder(EntityController entityController, Vector3 targetPosition, EntityController targetEntityController)
        {
            Debug.Log("Executing Test Non Targeted Order");
        }
    }
    public class TestPositionTargetedOrder : PositionTargetedOrder
    {
        public override string Name => "Test Position Targeted Order";
        public override string Description => "This is a test order that requires a position target.";
        public override Sprite Icon => null;
        public override Texture2D CursorTexture => null;

        public override void ExecuteOrder(EntityController entityController, Vector3 targetPosition, EntityController targetEntityController)
        {
            Debug.Log($"Executing Test Position Targeted Order at position {targetPosition}");
        }
    }
    public class TestEntityTargetedOrder : EntityTargetedOrder
    {
        public override string Name => "Test Entity Targeted Order";
        public override string Description => "This is a test order that requires an entity target.";
        public override Sprite Icon => null;
        public override Texture2D CursorTexture => null;
        public override List<TargetScope> TargetableScopes => new List<TargetScope> { TargetScope.Any };

        public override List<UnitType> TargetableUnitTypes => new List<UnitType> { /* Add targetable unit types here */ };
        public override List<BuildingType> TargetableBuildingTypes => new List<BuildingType> { /* Add targetable building types here */ };

        public override void ExecuteOrder(EntityController entityController, Vector3 targetPosition, EntityController targetEntityController)
        {
            Debug.Log($"Executing Test Entity Targeted Order on entity {targetEntityController.referenceEntity.name}");
        }
    }
    [System.Serializable]
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

        public override void ExecuteOrder(EntityController entityController, Vector3 targetPosition, EntityController targetEntityController)
        {
            entityController.MoveTo(targetPosition);
        }
    }

    /// <summary>Assigns a persistent Attack mission to the selected entity.</summary>
    [System.Serializable]
    public class AttackOrder : EntityTargetedOrder
    {
        [SerializeField] private string name = "Attack";
        [SerializeField] private string description = "Pursue and attack the selected enemy until it is destroyed.";
        [SerializeField] private Sprite icon;
        [SerializeField] private Texture2D cursorTexture;

        public override string Name => name;
        public override string Description => description;
        public override Sprite Icon => icon;
        public override Texture2D CursorTexture => cursorTexture;
        public override List<TargetScope> TargetableScopes => new List<TargetScope> { TargetScope.Enemy };
        public override List<UnitType> TargetableUnitTypes => new List<UnitType>();
        public override List<BuildingType> TargetableBuildingTypes => new List<BuildingType>();

        public override void ExecuteOrder(EntityController entityController, Vector3 targetPosition, EntityController targetEntityController)
        {
            entityController?.SetAttackTarget(targetEntityController);
        }
    }
}
