using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
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

        [UsingDependency]
        private static ILogger Logger;

        public static NetworkingClient client;

        public void SendToServer(INetworkMessage networkMessage)
        {
            if (client == null)
            {
                Logger.WriteLine("Attempted to send a message to the server while not connected.", LogType.WARNING);
                return;
            }
            client.QueueMessage(networkMessage);
        }

    }
}
