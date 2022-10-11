using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    /// <summary>
    /// Called to make an entity leave its parent container
    /// </summary>
    public class RemoveFromContentsEvent : IEvent
    {
    }
}
