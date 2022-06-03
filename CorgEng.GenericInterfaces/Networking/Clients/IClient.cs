using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Clients
{
    public interface IClient
    {

        /// <summary>
        /// The IP address of the client
        /// </summary>
        IPEndPoint ClientEndPoint { get; }

        /// <summary>
        /// The view details of the client
        /// </summary>
        ClientView View { get; }

        /// <summary>
        /// The username attached to this client
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Sends a message to this client.
        /// </summary>
        void SendMessage(UdpClient udpClient, byte[] message, int amount);

        /// <summary>
        /// Forcefully disconnect a client from the server
        /// </summary>
        void Disconnect(string disconnectReason);

        /// <summary>
        /// Ban a client from the server
        /// </summary>
        void Ban(string banReason);

    }
}
