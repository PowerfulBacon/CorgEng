using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Networking.Server
{
    [Dependency]
    internal class NetworkServerFactory : INetworkServerFactory
    {

        public INetworkServer CreateNetworkServer(IWorld world)
        {
            return new NetworkServer(world);
        }

    }
}
