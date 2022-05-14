using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking
{
    public interface IClient
    {

        /// <summary>
        /// The IP address of the client
        /// </summary>
        IPAddress ClientAddress { get; }

        /// <summary>
        /// The username attached to this client
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Sends a message to this client.
        /// </summary>
        void SendMessage(byte[] message);

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
