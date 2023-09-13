using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.EntityComponentSystem
{
    public interface IEntity
    {

        public delegate void InternalSignalHandleDelegate(IEntity entity, IEvent signal, bool synchronous, string callingFile, string callingMember, int callingLine);

        /// <summary>
        /// The name of the definition used to spawn this entity.
        /// Null if the entity was created without using EntityCreator.
        /// </summary>
        string? DefinitionName { get; set; }

        /// <summary>
        /// A list of all components attached to this entity.
        /// </summary>
        List<IComponent> Components { get; }

        /// <summary>
        /// A unique identifier for this entity.
        /// </summary>
        uint Identifier { get; }

        /// <summary>
        /// Flags relating to this entity.
        /// </summary>
        EntityFlags EntityFlags { get; set; }

        void AddComponent(IComponent component);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="networked"></param>
        /// <param name="silent">If set to true, will not raise component removed events.</param>
        void RemoveComponent(IComponent component, bool networked, bool silent = false);

        void HandleSignal(IEvent signal, bool synchronous, string callingFile, string callingMember, int callingLine);

        /// <summary>
        /// Gets a component of type. This is slow, avoid using it extensively.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetComponent<T>();

        /// <summary>
        /// Try and get a component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        bool TryGetComponent<T>(out T component);

        /// <summary>
        /// Attempt to find this component, returns null if unfound.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T? FindComponent<T>();

        /// <summary>
        /// Returns true if this object has the requested component type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasComponent<T>();

        /// <summary>
        /// Add an event listener to a specific type
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventHandler"></param>
        void AddEventListener(Type eventType, InternalSignalHandleDelegate eventHandler);

        /// <summary>
        /// Remove an event listener from a provided event type.
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventHandler"></param>
        void RemoveEventListenter(Type eventType, InternalSignalHandleDelegate eventHandler);

    }
}
