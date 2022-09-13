using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Claims.Events
{
    public class RequestClaimEvent : IEvent
    {

        /// <summary>
        /// The owner of this event
        /// </summary>
        public IEntity Owner { get; }

        /// <summary>
        /// Set to true if the claim was successful.
        /// Useful if you are calling this event synchronously.
        /// </summary>
        public bool ClaimSuccessful { get; set; } = false;

        /// <summary>
        /// Action to invoke when a claim was successful
        /// </summary>
        public Action? OnClaimSuccess { get; }

        /// <summary>
        /// Action invoked when a claim fails.
        /// </summary>
        public Action? OnClaimFailure { get; }

        public RequestClaimEvent(IEntity owner)
        {
            Owner = owner;
        }

    }
}
