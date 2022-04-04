using CorgEng.EntityComponentSystem.Entities;
using System;
using System.Collections.Generic;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;

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

        /// <summary>
        /// Raise the event globally
        /// </summary>
        public void RaiseGlobally()
        {
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(typeof(GlobalEventComponent)))
                return;
            //Locate all event types we are listening for
            foreach (Type eventType in EventManager.RegisteredEvents[typeof(GlobalEventComponent)])
            {
                EventComponentPair key = new EventComponentPair(eventType, typeof(GlobalEventComponent));
                //Locate the monitoring system's callback handler
                if (!RegisteredSystemSignalHandlers.ContainsKey(key))
                {
                    continue;
                }
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                foreach (SystemEventHandlerDelegate systemEventHandler in systemEventHandlers)
                    systemEventHandler.Invoke(null, null, this);
            }
        }

    }
}
