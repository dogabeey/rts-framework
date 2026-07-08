using UnityEngine;

namespace Game.Entity
{
    [RequireComponent(typeof(MovementController))]
    public class UnitController : EntityController
    {
        public Unit Unit => referenceEntity as Unit;

        public MovementController movementController;

        protected override void InitReferences()
        {
            base.InitReferences();
            movementController = GetComponent<MovementController>();
        }
    }
}
