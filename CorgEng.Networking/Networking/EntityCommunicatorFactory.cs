using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Networking
{
    [Dependency]
    internal class EntityCommunicatorFactory : IEntityCommunicatorFactory
    {
        public IEntityCommunicator CreateEntityCommunicator(IWorld world)
        {
            return new EntityCommunicator(world);
        }
    }
}
