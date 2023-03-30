using CorgEng.Core.Dependencies;
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

        [UsingDependency]
        private static IClientAddressFactory ClientAddressFactory;

        public static NetworkServer server;

        public bool IsServer => server != null;

        public void SendToClient(INetworkMessage networkMessage, IClient client)
        {
            if (server == null)
                return;
            server.QueueMessage(server.ClientAddressingTable.GetFlagRepresentation(client), networkMessage);
        }

        public void SendToClients(INetworkMessage networkMessage)
        {
            if (server == null)
                return;
            server.QueueMessage(server.ClientAddressingTable.GetEveryone(), networkMessage);
        }

        public void SendToReleventClients(INetworkMessage networkMessage, IVector<float> position, IVector<float> scale)
        {
            if (server == null)
                return;
            //Go thruogh each client and check if its in view
            //This is quicker than checking every tile in view and seeing if a client can see it
            IEnumerable<IClient> clientList = server.ClientAddressingTable.GetEveryone().GetClients();
            IClientAddress addressToSendTo = ClientAddressFactory.CreateEmptyAddress();
            foreach (IClient client in clientList)
            {
                //TODO: Client position
                addressToSendTo.EnableFlag(server.ClientAddressingTable.GetFlagRepresentation(client));
            }
            //Create the message
            server.QueueMessage(addressToSendTo, networkMessage);
        }

    }
}
