using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;

namespace CorgEng.EntityComponentSystem.Components
{
    internal static class ComponentExtensions
    {

        [UsingDependency]
        private static ILogger Logger;

        private static Dictionary<IComponent, List<InternalSignalHandleDelegate>> componentInjectionLambdas = new Dictionary<IComponent, List<InternalSignalHandleDelegate>>();

        /// <summary>
        /// Register existing signals when we are added
        /// to an entity.
        /// </summary>
        internal static void OnComponentAdded(this IComponent component, Entity parent)
        {
            component.Parent = parent;
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(component.GetType()))
            {
                //Send the component added event
                new ComponentAddedEvent(component).Raise(parent);
                return;
            }
            //Locate all event types we are listening for
            foreach (Type eventType in EventManager.RegisteredEvents[component.GetType()])
            {
                EventComponentPair key = new EventComponentPair(eventType, component.GetType());
                //Locate the monitoring system's callback handler
                if (!RegisteredSystemSignalHandlers.ContainsKey(key))
                {
                    continue;
                }
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                //Create a lambda function that injects this component and relays it to the system
                InternalSignalHandleDelegate componentInjectionLambda = (IEntity entity, IEvent signal, bool synchronous) =>
                {
                    for (int i = systemEventHandlers.Count - 1; i >= 0; i--)
                    {
                        systemEventHandlers[i].Invoke(entity, component, signal, synchronous);
                    }
                };
                if (!componentInjectionLambdas.ContainsKey(component))
                {
                    componentInjectionLambdas.Add(component, new List<InternalSignalHandleDelegate>());
                }
                lock (componentInjectionLambdas[component])
                {
                    componentInjectionLambdas[component].Add(componentInjectionLambda);
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
            ComponentAddedEvent componentAddedEvent = new ComponentAddedEvent(component);
            componentAddedEvent.Raise(parent);
        }

        /// <summary>
        /// Called when the component is removed from an entity.
        /// Cleanup registered signals and remove any static references to the entity.
        /// Allow for safe garbage collection
        /// </summary>
        internal static void OnComponentRemoved(this IComponent component, Entity parent)
        {
            component.Parent = null;
            //Raise component removed event.
            new ComponentRemovedEvent(component).Raise(parent);
            //Check if we have any registered signals
            if (!EventManager.RegisteredEvents.ContainsKey(component.GetType()))
                return;
            //Locate all event types we are listening for
            foreach (Type eventType in EventManager.RegisteredEvents[component.GetType()])
            {
                EventComponentPair key = new EventComponentPair(eventType, component.GetType());
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
                    lock (componentInjectionLambdas[component])
                    {
                        for (int i = componentInjectionLambdas[component].Count - 1; i >= 0; i--)
                        {
                            InternalSignalHandleDelegate signalHandleDelegate = componentInjectionLambdas[component][i];
                            if (parent.EventListeners[eventType].Contains(signalHandleDelegate))
                            {
                                parent.EventListeners[eventType].Remove(signalHandleDelegate);
                                componentInjectionLambdas[component].Remove(signalHandleDelegate);
                            }
                        }
                        if (componentInjectionLambdas[component].Count == 0)
                        {
                            componentInjectionLambdas.Remove(component);
                        }
                    }
                }
                else
                    Logger.WriteLine($"Parent event listener isn't listening for {eventType}", LogType.WARNING);
            }
        }

    }
}
