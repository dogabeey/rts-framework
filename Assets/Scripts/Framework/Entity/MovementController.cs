using UnityEngine;

namespace Game.Entity
{
    public class MovementController : MonoBehaviour
    {
        public MovementTags MovementTagList => movementTagList;
        public float Speed => baseSpeed;
        public float Acceleration => baseAcceleration;
        public float Deceleration => baseDeceleration;

        [SerializeField] private MovementTags movementTagList;
        [SerializeField] private float baseSpeed;
        [SerializeField] private float baseAcceleration;
        [SerializeField] private float baseDeceleration;

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
