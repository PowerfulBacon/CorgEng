using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using System;
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
        internal Dictionary<Type, List<InternalSignalHandleDelegate>> EventListeners { get; } = null;

        public void AddComponent(Component component)
        {
            Components.Add(component);
            component.OnComponentAdded(this);
        }

        /// <summary>
        /// Internal method for handling signals.
        /// </summary>
        /// <param name="signal"></param>
        internal void HandleSignal(Event signal)
        {
            //Verify that this signal is being listened for
            if (EventListeners == null)
                return;
            if (!EventListeners.ContainsKey(signal.GetType()))
                return;
            //Fetch the registered signal handlers
            List<InternalSignalHandleDelegate> signalHandleDelegates = EventListeners[signal.GetType()];
            //Call the signals
            foreach (InternalSignalHandleDelegate internalSignalHandler in signalHandleDelegates)
            {
                internalSignalHandler.Invoke(this, signal);
            }
        }

    }
}
