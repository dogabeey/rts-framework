using UnityEngine;

namespace Game.Entity
{
    [RequireComponent(typeof(MovementController), typeof(AttackableComponent), typeof(DamageableComponent))]
    public class UnitController : EntityController
    {
        public Unit Unit => referenceEntity as Unit;

        public MovementController movementController;
        public AttackableComponent attackableComponent;
        public DamageableComponent damageableComponent;

        void Start()
        {
            InitReferences();
        }
        private void InitReferences()
        {
            movementController = GetComponent<MovementController>();
            attackableComponent = GetComponent<AttackableComponent>();
            damageableComponent = GetComponent<DamageableComponent>();
        }
    }
}
