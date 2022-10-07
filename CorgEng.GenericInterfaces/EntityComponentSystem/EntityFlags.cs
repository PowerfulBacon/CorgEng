using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Entities
{
    public enum EntityFlags
    {
        /// <summary>
        /// No flags, default value
        /// </summary>
        NONE = 0,
        /// <summary>
        /// The entity has been initialised.
        /// Uninitialised entities will not have components setup properly.
        /// </summary>
        INITIALISED = 1 << 0,
        /// <summary>
        /// The entity has been destroyed, it should no longer be processing
        /// or receiving events at this point.
        /// </summary>
        DESTROYED = 1 << 1,
    }
}
