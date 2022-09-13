using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Claims.Events
{
    public class ReleaseClaimEvent : IEvent
    {

        /// <summary>
        /// The owner that is triggering this event
        /// </summary>
        public IEntity Owner { get; }

        public ReleaseClaimEvent(IEntity owner)
        {
            Owner = owner;
        }
    }
}
