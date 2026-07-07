using UnityEngine;

namespace Game.Entity
{
    public class UnitController : EntityController
    {
        public Unit Unit => referenceEntity as Unit;
    }
}
