using UnityEngine;

namespace Game.Entity
{
    public interface IMoveable
    {
        public float Speed { get; }
        public float Acceleration { get; }
        public float Deceleration { get; }
        public bool IsFlyer { get; }
        void Move(float deltaTime, Vector2 targetPosition);
        void AttackMove(float deltaTime, Vector2 targetPosition);
    }
}
