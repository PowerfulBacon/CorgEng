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
        
        bool SetupCompleted { get; }

        event Action postSetupAction;

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

    }
}
