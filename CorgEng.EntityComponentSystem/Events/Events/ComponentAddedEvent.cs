using CorgEng.EntityComponentSystem.Components;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class ComponentAddedEvent : Event
    {

        public Component Component { get; set; }

        public ComponentAddedEvent(Component component)
        {
            Component = component;
        }
    }
}
