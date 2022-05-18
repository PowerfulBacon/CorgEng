using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Clients
{
    [Dependency]
    internal class ClientAddressFactory : IClientAddressFactory
    {

        public IClientAddress CreateAddress(int clientId)
        {
            return new ClientAddress(clientId);
        }

        public IClientAddress CreateEmptyAddress()
        {
            return new ClientAddress(0);
        }

    }
}
