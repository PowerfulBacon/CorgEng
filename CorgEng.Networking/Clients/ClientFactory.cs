using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking;
using CorgEng.GenericInterfaces.Networking.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Clients
{
    [Dependency]
    internal class ClientFactory : IClientFactory
    {
        public IClient CreateClient(string username, IPEndPoint clientEndPoint)
        {
            return new Client(username, clientEndPoint);
        }
    }
}
