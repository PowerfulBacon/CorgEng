using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Networking.Server
{
    [Dependency]
    internal class ServerCommunicator : IServerCommunicator
    {

        public static NetworkingServer server;

        public bool IsServer => server != null;

        public void SendToClient(INetworkMessage networkMessage, IClient client)
        {
            server.QueueMessage(server.ClientAddressingTable.GetFlagRepresentation(client), networkMessage);
        }

        public void SendToClients(INetworkMessage networkMessage)
        {
            server.QueueMessage(server.ClientAddressingTable.GetEveryone(), networkMessage);
        }

        public void SendToReleventClients(INetworkMessage networkMessage, IVector<float> position, IVector<float> scale)
        {
            throw new NotImplementedException();
        }

    }
}
