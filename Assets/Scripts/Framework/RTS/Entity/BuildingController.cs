using UnityEngine;

namespace Game.RTS
{
    public class BuildingController : EntityController
    {
        public Building Building => referenceEntity as Building;

        protected override void UpdateEntityState()
        {
            base.UpdateEntityState();
        }
    }
}
