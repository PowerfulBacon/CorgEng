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

        void AddComponent(IComponent component);

        void RemoveComponent(IComponent component, bool networked);

        void HandleSignal(IEvent signal, bool synchronous = false);

        /// <summary>
        /// Gets a component of type. This is slow, avoid using it extensively.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetComponent<T>();

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

    }
}
