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
            if (movementController != null) movementController.referenceEntity = this;
        }

        protected override void UpdateEntityState()
        {
            if(movementController.HasMoveTarget)
            {
                EntityState = EntityState.moving;
            }
            else
            {
                EntityState = EntityState.idle;
            }
        }
    }
}
