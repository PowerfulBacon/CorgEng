using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.PrototypeManager
{
    public interface IPrototypeManager
    {

        /// <summary>
        /// Gets the prototype relating to a specific entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IPrototype GetPrototype(IEntity entity, bool network = true);

        /// <summary>
        /// Gets a prototype from a serialized byte array
        /// </summary>
        /// <param name="serializedPrototype"></param>
        /// <returns></returns>
        IPrototype GetPrototype(byte[] serializedPrototype);

        /// <summary>
        /// Get a prototype by its identifier
        /// </summary>
        /// <param name="prototypeIdentifier"></param>
        /// <returns></returns>
        IPrototype GetLocalProtoype(uint prototypeIdentifier);

        /// <summary>
        /// Add the prototype to the prototype manager
        /// </summary>
        /// <param name="prototype"></param>
        void AddPrototype(IPrototype prototype);

        /// <summary>
        /// Get the prototype from identifier.
        /// If the prototype hasn't been acquired from the server yet, this may need to wait for networking.
        /// </summary>
        /// <param name="prototypeIdentifier"></param>
        /// <returns></returns>
        Task<IPrototype> GetPrototype(uint prototypeIdentifier);

    }
}
