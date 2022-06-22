using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Networking.Server
{
    [Dependency]
    internal class EntityCommunicator : IEntityCommunicator
    {

        [UsingDependency]
        private static IPrototypeManager PrototypeManager;

        /// <summary>
        /// Communicate information about an entity to a client.
        /// We need to include:
        /// - Prototype ID
        /// - Any non-prototyped variables
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="target"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void CommunicateEntity(IEntity entity, IClient target)
        {
            throw new NotImplementedException();
        }

        private byte[] SerializeEntity(IEntity entity)
        {
            //Determine the size required
            int bytesRequired = sizeof(uint);

            //Add to a memory stream
            uint prototypeId = PrototypeManager.GetPrototype(entity).Identifier;
            throw new NotImplementedException();
        }

    }
}
