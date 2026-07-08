using System.Collections.Generic;
using Game.ModifierSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entity
{
    public class MovementController : MonoBehaviour
    {
        public MovementTags MovementTagList => movementTagList;
        public float Speed => baseSpeed;
        public float Acceleration => baseAcceleration;
        public float Deceleration => baseDeceleration;

        [SerializeField, FoldoutGroup("Movement")] private MovementTags movementTagList;
        [SerializeField, FoldoutGroup("Movement")] private float baseSpeed;
        [SerializeField, FoldoutGroup("Movement")] private float baseAcceleration;
        [SerializeField, FoldoutGroup("Movement")] private float baseDeceleration;
        [SerializeField, FoldoutGroup("Modifier")] private List<Modifier> movementModifiers = new List<Modifier>();

        public void Move(float deltaTime, Vector2 targetPosition)
        {
            // Implement movement logic here
            throw new System.NotImplementedException("Move method is not implemented yet.");
        }

        [System.Flags]
        public enum MovementTags
        {
            None = 0,
            Ground = 1 << 0,
            Air = 1 << 1,
            Water = 1 << 2,
            // Add more movement tags as needed
        }
    }
}
