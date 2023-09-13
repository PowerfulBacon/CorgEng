using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.VersionSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.PrototypeManager
{
    public interface IPrototype
    {

        /// <summary>
        /// The unique identifier of this prototype.
        /// 4 byte integer value. Allows for 4 billion prototypes, which should be
        /// basically impossible to reach.
        /// </summary>
        uint Identifier { get; }

        /// <summary>
        /// Generates this prototype based on a provided entity
        /// </summary>
        /// <param name="entity"></param>
        void GenerateFromEntity(IEntity entity);

        /// <summary>
        /// Create a new entity from a prototype, with a new and unique identifier.
        /// </summary>
        /// <returns></returns>
        IEntity CreateEntityFromPrototype(IWorld world);

        /// <summary>
        /// Create an existing entity based on the information in this prototype
        /// </summary>
        /// <returns>An instantiated entity based on the contents of this prototype.</returns>
        IEntity CreateEntityFromPrototype(IWorld world, uint entityIdentifier);

        /// <summary>
        /// Serializes the prototype into a byte array, allowing it to be transmitted through the networking.
        /// </summary>
        /// <returns>A byte array representing this prototype.</returns>
        byte[] SerializePrototype();

        /// <summary>
        /// Deserializes a byte array to get information about this prototype.
        /// </summary>
        /// <param name="data">The data to deserialize</param>
        void DeserializePrototype(byte[] data);

    }
}
