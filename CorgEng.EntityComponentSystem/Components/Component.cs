using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components.ComponentVariables;
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
using System.Linq;
using System.Reflection;
using static CorgEng.EntityComponentSystem.Entities.Entity;
using static CorgEng.EntityComponentSystem.Systems.EntitySystem;
using static CorgEng.GenericInterfaces.EntityComponentSystem.IEntity;

namespace CorgEng.EntityComponentSystem.Components
{

    public abstract class Component : IInstantiatable, IVersionSynced, IComponent
    {

        [UsingDependency]
        private static ILogger Logger = null!;

        /// <summary>
        /// The parent of this component
        /// </summary>
        public IEntity Parent { get; set; }

        public IEntityDefinition TypeDef { get; set; }

        public virtual bool IsSynced { get; } = true;

        //TODO: This is very memory expensive as its stored on ALL component instances, when it kind of works per-component.
        private List<InternalSignalHandleDelegate> componentInjectionLambdas = new List<InternalSignalHandleDelegate>();

        private static Dictionary<Type, PropertyInfo[]> cvarTypeCache = new Dictionary<Type, PropertyInfo[]>();

        public Component()
        {
            if (!cvarTypeCache.ContainsKey(GetType()))
            {
                lock (cvarTypeCache)
                {
                    if (!cvarTypeCache.ContainsKey(GetType()))
                    {
                        cvarTypeCache.Add(GetType(), GetType()
                            .GetProperties()
                            .Where(x => typeof(IComponentVariable).IsAssignableFrom(x.PropertyType))
                            .ToArray());
                    }
                }
            }
            foreach (PropertyInfo cvarProperty in cvarTypeCache[GetType()])
            {
                IComponentVariable attachedCVar = (IComponentVariable)cvarProperty.GetValue(this);
                attachedCVar.AssociateTo(this);
            }
        }

        /// <summary>
        /// Super cool optimisation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class CompTemplate<T>
            where T : Component
        {
            internal static readonly T Template = Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Get the template component so that signals can be registered to its CVars
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetTemplate<T>()
            where T : Component
        {
            return CompTemplate<T>.Template;
        }

        /// <summary>
        /// Register existing signals when we are added
        /// to an entity.
        /// </summary>
        internal void OnComponentAdded(Entity parent)
        {
            //Locate all event types we are listening for
            foreach (Type eventType in parent.world.EntitySystemManager.GetRegisteredEventTypes(GetType()))
            {
                EventComponentPair key = new EventComponentPair(eventType, GetType());
                //Locate the monitoring system's callback handler
                if (!RegisteredSystemSignalHandlers.ContainsKey(key))
                {
                    continue;
                }
                List<SystemEventHandlerDelegate> systemEventHandlers = RegisteredSystemSignalHandlers[key];
                //Create a lambda function that injects this component and relays it to the system
                InternalSignalHandleDelegate componentInjectionLambda = (IEntity entity, IEvent signal, bool synchronous, string callingFile, string callingMember, int callingLine) => {
                    foreach(SystemEventHandlerDelegate systemEventHandler in systemEventHandlers)
                        systemEventHandler.Invoke(entity, this, signal, synchronous, callingFile, callingMember, callingLine);
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
            //Send component added event if already initialised
            if ((parent.EntityFlags & EntityFlags.INITIALISED) != 0)
            {
                //Send the component added event
                ComponentAddedEvent componentAddedEvent = new ComponentAddedEvent(this);
                componentAddedEvent.Raise(parent);
            }
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
            //Locate all event types we are listening for
            foreach (Type eventType in parent.world.EntitySystemManager.GetRegisteredEventTypes(GetType()))
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
