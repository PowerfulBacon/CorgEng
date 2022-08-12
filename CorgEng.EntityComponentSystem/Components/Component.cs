using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.VersionSync;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;

namespace CorgEng.EntityComponentSystem.Components
{
    public abstract class Component : IInstantiatable, IVersionSynced, IComponent
    {

        [UsingDependency]
        private static ILogger Logger;
        
        /// <summary>
        /// The parent of this component
        /// </summary>
        public IEntity Parent { get; set; }

        public IEntityDefinition TypeDef { get; set; }

        public virtual bool IsSynced { get; } = true;

        private List<InternalSignalHandleDelegate> componentInjectionLambdas = new List<InternalSignalHandleDelegate>();

        /// <summary>
        /// Register existing signals when we are added
        /// to an entity.
        /// </summary>
        internal void OnComponentAdded(Entity parent)
        {
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(GetType()))
            {
                //Send the component added event
                new ComponentAddedEvent(this).Raise(parent);
                return;
            }
            //Locate all event types we are listening for
            foreach (Type eventType in EventManager.RegisteredEvents[GetType()])
            {
                EventComponentPair key = new EventComponentPair(eventType, GetType());
                //Locate the monitoring system's callback handler
                if (!RegisteredSystemSignalHandlers.ContainsKey(key))
                {
                    continue;
                }
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                //Create a lambda function that injects this component and relays it to the system
                InternalSignalHandleDelegate componentInjectionLambda = (IEntity entity, IEvent signal, bool synchronous) => {
                    foreach(SystemEventHandlerDelegate systemEventHandler in systemEventHandlers)
                        systemEventHandler.Invoke(entity, this, signal, synchronous);
                };
                lock (componentInjectionLambdas)
                {
                    componentInjectionLambdas.Add(componentInjectionLambda);
                }
                //Start listening for this event
                if (parent.EventListeners == null)
                    parent.EventListeners = new Dictionary<Type, List<InternalSignalHandleDelegate>>();
                if (parent.EventListeners.ContainsKey(eventType))
                    parent.EventListeners[eventType].Add(componentInjectionLambda);
                else
                    parent.EventListeners.Add(eventType, new List<InternalSignalHandleDelegate>() { componentInjectionLambda });
            }
            //Send the component added event
            ComponentAddedEvent componentAddedEvent = new ComponentAddedEvent(this);
            componentAddedEvent.Raise(parent);
        }

        /// <summary>
        /// Called when the component is removed from an entity.
        /// Cleanup registered signals and remove any static references to the entity.
        /// Allow for safe garbage collection
        /// </summary>
        internal void OnComponentRemoved(Entity parent)
        {
            //Raise component removed event.
            new ComponentRemovedEvent(this).Raise(parent);
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
                //Start listening for this event
                if (parent.EventListeners == null)  //Probably shouldn't happen
                {
                    Logger.WriteLine($"Parent event listeners is null.", LogType.WARNING);
                    continue;
                }
                //Locate and removed
                if (parent.EventListeners.ContainsKey(eventType))
                {
                    lock (componentInjectionLambdas)
                    {
                        for (int i = componentInjectionLambdas.Count - 1; i >= 0; i--)
                        {
                            InternalSignalHandleDelegate signalHandleDelegate = componentInjectionLambdas[i];
                            if (parent.EventListeners[eventType].Contains(signalHandleDelegate))
                            {
                                parent.EventListeners[eventType].Remove(signalHandleDelegate);
                                componentInjectionLambdas.Remove(signalHandleDelegate);
                            }
                        }
                    }
                }
                else
                    Logger.WriteLine($"Parent event listener isn't listening for {eventType}", LogType.WARNING);
            }
        }

    }
}
