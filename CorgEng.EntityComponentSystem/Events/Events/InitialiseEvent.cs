using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    /// <summary>
    /// Called after an entity has been fully initialised, with the correct transform position.
    /// </summary>
    public class InitialiseEvent : IEvent
    {
    }
}
