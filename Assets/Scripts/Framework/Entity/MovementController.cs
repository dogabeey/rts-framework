using UnityEngine;

namespace Game.Entity
{
    public class MovementController : MonoBehaviour
    {
        public float Speed { get; }
        public float Acceleration { get; }
        public float Deceleration { get; }
        public bool IsFlyer { get; }
        public void Move(float deltaTime, Vector2 targetPosition)
        {
            // Implement movement logic here
            throw new System.NotImplementedException("Move method is not implemented yet.");
        }
    }
}
