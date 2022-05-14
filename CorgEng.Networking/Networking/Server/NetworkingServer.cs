using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Networking.Networking.Server
{
    [Dependency]
    internal class NetworkingServer : INetworkingServer
    {

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// The client we are using to communicate
        /// </summary>
        private UdpClient udpClient;

        /// <summary>
        /// Mark false if the server should shut down
        /// </summary>
        private volatile bool running = true;

        /// <summary>
        /// The port the server is currently running on.
        /// </summary>
        private int port;

        /// <summary>
        /// A dictionary containing all connected clients
        /// </summary>
        private Dictionary<IPAddress, IClient> connectedClients = new Dictionary<IPAddress, IClient>();

        /// <summary>
        /// Set the server transmission tick rate
        /// </summary>
        public int TickRate { get; set; } = 32;

        public void StartHosting(int port)
        {
            //We are already connected to the UDP client
            if (udpClient != null && udpClient.Client.Connected)
            {
                throw new Exception($"Server on port {port} is already running.");
            }
            //Log
            Logger?.WriteLine($"Starting UDP server on port {port}.", LogType.MESSAGE);
            //Set the listening port
            this.port = port;
            //Start the server (Doesn't connect to anything, just listens)
            udpClient = new UdpClient(port);
            //Start the networking thread
            Thread serverThread = new Thread(NetworkingServerThread);
            serverThread.Name = $"Networking server ({port})";
            serverThread.Start();
        }

        private void NetworkingServerThread()
        {
            while (running)
            {
                try
                {
                    //Recieve messages
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
                    byte[] incomingData = udpClient.Receive(ref remoteEndPoint);
                    Task.Run(() => HandleMessage(remoteEndPoint, incomingData));
                }
                catch (Exception e)
                {
                    Logger?.WriteLine($"Critical exception on server: {e}", LogType.ERROR);
                }
            }
            //Log shutdown
            Logger?.WriteLine($"Networking server shut down.", LogType.MESSAGE);
        }

        private void HandleMessage(IPEndPoint sender, byte[] message)
        {
            //Check the message header
            //Process messages
            int packetHeader = BitConverter.ToInt32(message, 0);
            if (connectedClients.ContainsKey(sender.Address))
            {

            }
            else
            {
                //Switch the message
                switch ((PacketHeaders)packetHeader)
                {
                    case PacketHeaders.CONNECTION_REQUEST:
                        HandleConnectionRequest(sender);
                        return;
                }
            }
        }

        private void HandleConnectionRequest(IPEndPoint sender)
        {
            //Just accept it for now
            Logger?.WriteLine($"Accepting connection from {sender}", LogType.DEBUG);
            //Create connection packet
            byte[] connectionPacket = BitConverter.GetBytes((int)PacketHeaders.CONNECTION_ACCEPT);
            udpClient.Send(connectionPacket, connectionPacket.Length, sender);
        }

        public void QueueMessage(INetworkMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
