using CorgEng.GenericInterfaces.EntityComponentSystem;
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
        /// The entity that we are attached to
        /// </summary>
        IEntity AttachedEntity { get; set; }

        /// <summary>
        /// The username attached to this client
        /// </summary>
        string Username { get; }

        /// <summary>
        /// The last time of the round trip ping
        /// </summary>
        public double RoundTripPing { get; set; }

        /// <summary>
        /// The number of pings that were missed, if too many are hit then the client is assumed disconnected
        /// </summary>
        public int PingsMissed { get; set; }

        /// <summary>
        /// The amount of packets that have been sent to this client
        /// </summary>
        public int PacketsSent { get; set; }

        /// <summary>
        /// The amount of packets that timed out and had to be resent
        /// </summary>
        public int PacketsDropped { get; set; }

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
