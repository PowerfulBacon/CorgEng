using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Networking.Client
{

    public delegate void ConnectionSuccess(IPAddress ipAddress);

    public delegate void ConnectionFailed(IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText);

    public interface INetworkingClient
    {

        /// <summary>
        /// The event called when a connection is succesfully established with the remote server.
        /// </summary>
        event ConnectionSuccess OnConnectionSuccess;

        /// <summary>
        /// The event called when connecting to the server fails
        /// </summary>
        event ConnectionFailed OnConnectionFailed;

        /// <summary>
        /// Called every time a network message is received
        /// </summary>
        event NetworkMessageRecieved NetworkMessageReceived;

        /// <summary>
        /// Attempt connection to a remote server
        /// </summary>
        /// <param name="address">The address of the server to connect to</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="timeout">The timeout time of the server</param>
        void AttemptConnection(string address, int port, int timeout = 5000);

        /// <summary>
        /// Queue message for transmission to the server
        /// </summary>
        void QueueMessage(IClientAddress targets, INetworkMessage message);

        /// <summary>
        /// Disconnect the client and cleanup any client stored stuff
        /// </summary>
        void Cleanup();

    }
}
