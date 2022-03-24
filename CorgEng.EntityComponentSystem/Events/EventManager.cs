using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events
{
    public static class EventManager
    {

        //Associates a type (type of component) to a list of types (event types)
        //that a component needs to be registered to when created.
        internal static Dictionary<Type, List<Type>> RegisteredComponentEvents { get; }

        //Associates a type (Type of event) to a list of component instances
        //WARNING:
        //This raises the event to ALL components that are listening rather than a specific component
        internal static Dictionary<Type, Component> RegisteredEvents { get; }

        internal static Dictionary<, Component>

        //Starts tracking components to events, so the component can be referenced
        //when an event is raised.
        public static void OnComponentAdded(Component component)
        {
            Type componentType = component.GetType();
            List<Type> joiningEvents = RegisteredComponentEvents[componentType];
            foreach (Type eventType in joiningEvents)
            {
                RegisteredEvents.Add(eventType, component);
            }
        }



    }
}
