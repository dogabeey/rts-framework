using UnityEngine;

namespace Game.Entity
{
    [RequireComponent(typeof(MovementController), typeof(AttackableComponent), typeof(DamageableComponent))]
    public class UnitController : EntityController
    {
        public Unit Unit => referenceEntity as Unit;
    }
}
