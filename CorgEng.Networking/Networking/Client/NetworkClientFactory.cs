using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Networking.Client
{
    [Dependency]
    internal class NetworkClientFactory : INetworkClientFactory
    {

        public INetworkClient CreateNetworkClient(IWorld world)
        {
            return new NetworkClient(world);
        }

    }
}
