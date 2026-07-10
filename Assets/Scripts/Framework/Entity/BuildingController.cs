using UnityEngine;

namespace Game.Entity
{
    public class BuildingController : EntityController
    {
        public Building Building => referenceEntity as Building;

        protected override void UpdateEntityState()
        {
            throw new System.NotImplementedException();
        }
    }
}
