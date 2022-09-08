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

        List<IComponent> Components { get; }

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

    }
}
