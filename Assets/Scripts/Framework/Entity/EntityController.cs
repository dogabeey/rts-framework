using UnityEngine;

namespace Game.Entity
{
    public abstract class EntityController : MonoBehaviour
    {
        [HideInInspector]
        public Entity referenceEntity;
    }
}
