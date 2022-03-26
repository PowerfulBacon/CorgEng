using CorgEng.Core.Logging;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using System;
using System.Collections.Generic;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;

namespace CorgEng.EntityComponentSystem.Components
{
    public abstract class Component
    {
        
        /// <summary>
        /// The parent of this component
        /// </summary>
        public Entity Parent { get; internal set; }

        /// <summary>
        /// Register existing signals when we are added
        /// to an entity.
        /// </summary>
        internal void OnComponentAdded(Entity parent)
        {
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(GetType()))
                return;
            //Locate all event types we are listening for
            foreach (Type eventType in EventManager.RegisteredEvents[GetType()])
            {
                EventComponentPair key = new EventComponentPair(eventType, GetType());
                //Locate the monitoring system's callback handler
                if (!RegisteredSystemSignalHandlers.ContainsKey(key))
                    continue;
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                //Create a lambda function that injects this component and relays it to the system
                InternalSignalHandleDelegate componentInjectionLambda = (Entity entity, Event signal) => {
                    foreach(SystemEventHandlerDelegate systemEventHandler in systemEventHandlers)
                        systemEventHandler.Invoke(entity, this, signal);
                };
                //Start listening for this event
                if (parent.EventListeners.ContainsKey(eventType))
                    parent.EventListeners[eventType].Add(componentInjectionLambda);
                else
                    parent.EventListeners.Add(eventType, new List<Entity.InternalSignalHandleDelegate>() { componentInjectionLambda });
            }
        }

    }
}
