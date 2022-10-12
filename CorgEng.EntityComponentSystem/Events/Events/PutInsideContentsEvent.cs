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
    /// Called on a child to put it inside another entity.
    /// </summary>
    public class PutInsideContentsEvent : IEvent
    {
        public IEntity NewHolder { get; }

        public PutInsideContentsEvent(IEntity holder)
        {
            NewHolder = holder;
        }
    }
}
