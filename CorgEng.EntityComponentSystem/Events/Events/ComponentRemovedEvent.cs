using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class ComponentRemovedEvent : IEvent
    {

        public IComponent Component { get; set; }

        public ComponentRemovedEvent(IComponent component)
        {
            Component = component;
        }
    }
}
