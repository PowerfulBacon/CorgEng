using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.EntityComponentSystem;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class ComponentAddedEvent : IEvent
    {

        public IComponent Component { get; set; }

        public ComponentAddedEvent(IComponent component)
        {
            Component = component;
        }
    }
}
