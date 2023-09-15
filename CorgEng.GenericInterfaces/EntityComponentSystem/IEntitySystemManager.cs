using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface IEntitySystemManager
    {

        /// <summary>
        /// The delegate used for handling system events.
        /// </summary>
        /// <param name="entity">The entity target of the signal handler.</param>
        /// <param name="component">The component that is affected by the raised signal.</param>
        /// <param name="signal">The signal that was raised.</param>
        /// <param name="synchronous">Boolean value which represents if the event was raised synchronously. Synchronous events
        /// will halt the curren threads execution until it is handled.</param>
        /// <param name="file">The name of the file that the event was called from.</param>
        /// <param name="member">The name of the member that raised the event.</param>
        /// <param name="lineNumber">The line number that the event was raised on.</param>
        public delegate void SystemEventHandlerDelegate(IEntity entity, IComponent component, IEvent signal, bool synchronous, string file, string member, int lineNumber);

        /// <summary>
        /// A boolean value representing wether or not the entity systems have been
        /// setup or not.
        /// </summary>
        bool SetupCompleted { get; }

        /// <summary>
        /// An event that is raised once the entity systems are registered and have had
        /// their SetupSystem function called. This will only ever be called once in normal
        /// operations, once it has been called adding to this event will be pointless as
        /// it will never be called again.
        /// </summary>
        event Action postSetupAction;

        /// <summary>
        /// Get the singleton subsystem with the type T,
        /// running within this world instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetSingleton<T>();

        /// <summary>
        /// Register an event with a specific type
        /// </summary>
        void RegisterEventType(Type componentType, Type eventType);

        /// <summary>
        /// Unregister an event with a given type for a given component.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        void UnregisterEventType(Type componentType, Type eventType);

        /// <summary>
        /// Returns the enumable set of types registered to the
        /// provided component type.
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        IEnumerable<Type> GetRegisteredEventTypes(Type componentType);

        /// <summary>
        /// Register a system event handler to this system manager group. When an event
        /// is raised on an object that contains a component of the types specified in the
        /// EventComponentPair, then the provided SystemEventHandlerDelegate will be raised.
        /// </summary>
        /// <param name="registeredEventComponentType"></param>
        /// <param name="eventAction"></param>
        void RegisterSystemEventHandler(EventComponentPair registeredEventComponentType, SystemEventHandlerDelegate eventAction);

        /// <summary>
        /// Unregister a system event handler attached to this system group. See RegisterSystemEventHandler
        /// for details.
        /// </summary>
        /// <param name="registeredEventComponentType"></param>
        /// <param name="eventAction"></param>
        void UnregisterSystemEventHandler(EventComponentPair registeredEventComponentType, SystemEventHandlerDelegate eventAction);

        /// <summary>
        /// Get an enumerable set of the registered system event handlers registered to a
        /// specified event component pair.
        /// </summary>
        /// <returns></returns>
        List<SystemEventHandlerDelegate> GetRegisteredSystemEventHandlers(EventComponentPair registeredEventComponentType);

        /**
         * Shutdown and cleanup
         */
        void Cleanup();

    }
}
