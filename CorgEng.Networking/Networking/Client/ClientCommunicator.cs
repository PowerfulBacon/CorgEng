using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Networking.Client
{
    [Dependency]
    internal class ClientCommunicator : IClientCommunicator
    {

        public static NetworkingClient client;

        public void SendToServer(INetworkMessage networkMessage)
        {
            client.QueueMessage(networkMessage);
        }

    }
}
