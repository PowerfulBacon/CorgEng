using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Clients
{
    public interface IClientFactory
    {

        /// <summary>
        /// Create a client with specified IP address
        /// </summary>
        IClient CreateClient(string username, IPEndPoint clientEndPoint);

    }
}
