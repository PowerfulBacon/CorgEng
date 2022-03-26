using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using System.Collections.Generic;

namespace CorgEng.EntityComponentSystem.Entities
{
    public class Entity
    {

        internal delegate void InternalSignalHandleDelegate(Entity entity, Event signal);

        /// <summary>
        /// List of all components attached to this entity
        /// </summary>
        public List<Component> Components { get; } = new List<Component>();

        /// <summary>
        /// Components register themselves to this when needed, and this gets fired off to the component
        /// which then can fire off itself to the system.
        /// </summary>
        internal Dictionary<EventComponentPair, List<InternalSignalHandleDelegate>> EventListeners { get; } = null;

        /// <summary>
        /// Internal method for handling signals.
        /// </summary>
        /// <typeparam name="ComponentTarget"></typeparam>
        /// <param name="signal"></param>
        internal void HandleSignal<ComponentTarget>(Event signal)
        {
            //Verify that this signal is being listened for
            if (EventListeners == null)
                return;
            EventComponentPair eventComponentPair = new EventComponentPair(signal.GetType(), typeof(ComponentTarget));
            if (!EventListeners.ContainsKey(eventComponentPair))
                return;
            //Fetch the registered signal handlers
            List<InternalSignalHandleDelegate> signalHandleDelegates = EventListeners[eventComponentPair];
            //Call the signals
            foreach (InternalSignalHandleDelegate internalSignalHandler in signalHandleDelegates)
            {
                internalSignalHandler.Invoke(this, signal);
            }
        }

    }
}
