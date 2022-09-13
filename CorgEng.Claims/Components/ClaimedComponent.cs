using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Claims.Components
{
    /// <summary>
    /// Component attached to an entity to indicate that it has been claimed by something.
    /// </summary>
    public class ClaimedComponent : Component
    {

        /// <summary>
        /// The owner of this entity
        /// </summary>
        public IEntity Owner { get; }

        public ClaimedComponent(IEntity owner)
        {
            Owner = owner;
        }
    }
}
