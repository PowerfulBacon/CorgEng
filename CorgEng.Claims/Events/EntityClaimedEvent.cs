using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Claims.Events
{
    public class EntityClaimedEvent : IEvent
    {

        /// <summary>
        /// The new owner of this entity
        /// </summary>
        public IEntity Owner { get; }

        public EntityClaimedEvent(IEntity owner)
        {
            Owner = owner;
        }
    }
}
