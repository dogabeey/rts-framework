using UnityEngine;

namespace Game.Entity
{
    public class EntityAnimationController : MonoBehaviour
    {
        public enum AnimationState
        {
            Idle,
            Moving,
            Attacking,
            Dying
        }
    }
}
