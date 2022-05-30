using CorgEng.EntityComponentSystem.Components;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class ComponentAddedEvent : Event
    {

        public Component Component { get; set; }

        public override bool IsSynced => false;

        public ComponentAddedEvent(Component component)
        {
            Component = component;
        }

    }
}
