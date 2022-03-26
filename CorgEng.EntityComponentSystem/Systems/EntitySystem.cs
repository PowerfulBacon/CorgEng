using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using System;
using System.Collections.Generic;

namespace CorgEng.EntityComponentSystem.Systems
{
    public abstract class EntitySystem
    {

        internal delegate void SystemEventHandlerDelegate(Entity entity, Component component, Event signal);

        /// <summary>
        /// Matches event and component types to registered signal handlers on systems
        /// </summary>
        internal static Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>> RegisteredSystemSignalHandlers { get; } = new Dictionary<EventComponentPair, List<SystemEventHandlerDelegate>>();

        public abstract void SystemSetup();

        /// <summary>
        /// Register to a local event
        /// </summary>
        public void RegisterLocalEvent<GComponent, GEvent>(Action<Entity, GComponent, GEvent> eventHandler)
            where GComponent : Component
            where GEvent : Event
        {
            //Register the component to recieve the target event on the event manager
            if (!EventManager.RegisteredEvents.ContainsKey(typeof(GComponent)))
                EventManager.RegisteredEvents.Add(typeof(GComponent), new List<Type>());
            if(!EventManager.RegisteredEvents[typeof(GComponent)].Contains(typeof(GEvent)))
                EventManager.RegisteredEvents[typeof(GComponent)].Add(typeof(GEvent));
            //Register the system to receieve the event
            EventComponentPair eventComponentPair = new EventComponentPair(typeof(GEvent), typeof(GComponent));
            if (!RegisteredSystemSignalHandlers.ContainsKey(eventComponentPair))
                RegisteredSystemSignalHandlers.Add(eventComponentPair, new List<SystemEventHandlerDelegate>());
            RegisteredSystemSignalHandlers[eventComponentPair].Add((Entity entity, Component component, Event signal) => {
                eventHandler.Invoke(entity, (GComponent)component, (GEvent)signal);
            });
        }

    }
}
