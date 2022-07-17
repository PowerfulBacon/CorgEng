using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Networking
{
    public interface IServerCommunicator
    {

        /// <summary>
        /// Are we hosting a server?
        /// </summary>
        bool IsServer { get; }

        /// <summary>
        /// Send a message to the clients.
        /// Sends this message to all connected clients
        /// </summary>
        /// <param name="networkMessage">The message to send to all clients.</param>
        void SendToClients(INetworkMessage networkMessage);

        /// <summary>
        /// Send to clients whom this may concern
        /// </summary>
        void SendToReleventClients(INetworkMessage networkMessage, IVector<float> position, IVector<float> scale);

        /// <summary>
        /// Send a message to a particular client
        /// </summary>
        /// <param name="networkMessage"></param>
        /// <param name="client"></param>
        void SendToClient(INetworkMessage networkMessage, IClient client);

    }
}
