using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;
using static CorgEng.GenericInterfaces.EntityComponentSystem.IEntity;

namespace CorgEng.EntityComponentSystem.Components
{
    internal class ComponentSignalInjector : WorldObject, IComponentSignalInjector
    {

        [UsingDependency]
        private static ILogger Logger;

        private ConcurrentDictionary<IComponent, List<InternalSignalHandleDelegate>> componentInjectionLambdas = new ConcurrentDictionary<IComponent, List<InternalSignalHandleDelegate>>();

        public ComponentSignalInjector(IWorld world) : base(world)
        {
        }

        /// <summary>
        /// Register existing signals when we are added
        /// to an entity.
        /// </summary>
        public void OnComponentAdded(IComponent component, IEntity parent)
        {
            component.Parent = parent;
            //Locate all event types we are listening for
            foreach (Type eventType in world.EntitySystemManager.GetRegisteredEventTypes(component.GetType()))
            {
                EventComponentPair key = new EventComponentPair(eventType, component.GetType());
                //Locate the monitoring system's callback handler
                if (!RegisteredSystemSignalHandlers.ContainsKey(key))
                {
                    continue;
                }
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                //Create a lambda function that injects this component and relays it to the system
                InternalSignalHandleDelegate componentInjectionLambda = (IEntity entity, IEvent signal, bool synchronous, string callingFile, string callingMember, int callingLine) =>
                {
                    for (int i = systemEventHandlers.Count - 1; i >= 0; i--)
                    {
                        systemEventHandlers[i].Invoke(entity, component, signal, synchronous, callingFile, callingMember, callingLine);
                    }
                };
                if (!componentInjectionLambdas.ContainsKey(component))
                {
                    componentInjectionLambdas.TryAdd(component, new List<InternalSignalHandleDelegate>());
                }
                lock (componentInjectionLambdas[component])
                {
                    componentInjectionLambdas[component].Add(componentInjectionLambda);
                }
                //Start listening for this event
                parent.AddEventListener(eventType, componentInjectionLambda);
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
        public void OnComponentRemoved(IComponent component, IEntity parent, bool silent)
        {
            component.Parent = null;
            //Raise component removed event.
            if (!silent)
            {
                new ComponentRemovedEvent(component).Raise(parent);
            }
            //Locate all event types we are listening for
            foreach (Type eventType in world.EntitySystemManager.GetRegisteredEventTypes(component.GetType()))
            {
                EventComponentPair key = new EventComponentPair(eventType, component.GetType());
                //Locate the monitoring system's callback handler
                if (!RegisteredSystemSignalHandlers.ContainsKey(key))
                    continue;
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                //Locate and removed
                lock (componentInjectionLambdas[component])
                {
                    for (int i = componentInjectionLambdas[component].Count - 1; i >= 0; i--)
                    {
                        InternalSignalHandleDelegate signalHandleDelegate = componentInjectionLambdas[component][i];
                        parent.RemoveEventListenter(eventType, signalHandleDelegate);
                        if (componentInjectionLambdas.ContainsKey(component))
                            componentInjectionLambdas[component].Remove(signalHandleDelegate);
                    }
                    if (componentInjectionLambdas[component].Count == 0)
                    {
                        componentInjectionLambdas.TryRemove(component, out _);
                    }
                }
            }
        }

    }
}
