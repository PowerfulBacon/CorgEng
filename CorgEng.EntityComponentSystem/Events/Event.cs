using CorgEng.EntityComponentSystem.Entities;

namespace CorgEng.EntityComponentSystem.Events
{
    public abstract class Event
    {

        /// <summary>
        /// Raise this event against a specified target
        /// </summary>
        public void Raise(Entity target)
        {
            target.HandleSignal(this);
        }

    }
}
