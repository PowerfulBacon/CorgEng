using CorgEng.Claims.Components;
using CorgEng.Claims.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Claims.Systems
{
    internal class ClaimSystem : EntitySystem
    {

        public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

        public override void SystemSetup(IWorld world)
        {
            //Deletable is common to all entities, so register to that component.
            RegisterLocalEvent<DeleteableComponent, RequestClaimEvent>(HandleClaimRequest);
        }

        /// <summary>
        /// Handle a claim request over an object
        /// </summary>
        private void HandleClaimRequest(IEntity entity, DeleteableComponent deleteableComponent, RequestClaimEvent requestClaimEvent)
        {
            //Ensure an owner is provided.
            if (requestClaimEvent.Owner == null)
            {
                throw new Exception("Attempting to claim an entity without an owner.");
            }
            //Check for existing claims
            if (entity.HasComponent<ClaimedComponent>())
            {
                //Invoke failure and return
                requestClaimEvent.OnClaimFailure?.Invoke();
                return;
            }
            //Apply claim
            entity.AddComponent(new ClaimedComponent(requestClaimEvent.Owner));
            requestClaimEvent.ClaimSuccessful = true;
            requestClaimEvent.OnClaimSuccess?.Invoke();
        }

    }
}
