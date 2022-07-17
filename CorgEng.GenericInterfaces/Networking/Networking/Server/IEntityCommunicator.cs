using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Networking.Server
{

    public interface IEntityCommunicator
    {

        /// <summary>
        /// Communicate information about an entity to a specific client.
        /// Will send over the prototype if required, otherwise just transmits the prototype ID and
        /// non prototyped values such as position (Position will always be different for all entities).
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="target"></param>
        void CommunicateEntity(IEntity entity, IClient target);

        /// <summary>
        /// Perform serialisation of the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        byte[] SerializeEntity(IEntity entity);

        /// <summary>
        /// Deserialise a byte array and create an entity based no the data in this byte array.
        /// </summary>
        /// <param name="data">The data to deserialise</param>
        /// <returns>Returns the created entity</returns>
        Task<IEntity> DeserialiseEntity(byte[] data);

    }

}
