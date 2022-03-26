﻿using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using System;
using System.Collections.Generic;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.System;

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
                SystemEventHandlerDelegate systemEventHandler = null;
                //Create a lambda function that injects this component and relays it to the system
                InternalSignalHandleDelegate componentInjectionLambda = (Entity entity, Event signal) => {
                    systemEventHandler.Invoke(entity, this, signal);
                };
                //Start listening for this event
                if (parent.EventListeners.ContainsKey(key))
                    parent.EventListeners[key].Add(componentInjectionLambda);
                else
                    parent.EventListeners.Add(key, new List<Entity.InternalSignalHandleDelegate>() { componentInjectionLambda });
            }
        }

    }
}
