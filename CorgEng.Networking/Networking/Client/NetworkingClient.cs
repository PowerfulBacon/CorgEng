using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Networking.Networking.Client
{
    [Dependency]
    internal class NetworkingClient : INetworkingClient
    {

        [UsingDependency]
        private static ILogger Logger;

        public event ConnectionSuccess OnConnectionSuccess;

        public event ConnectionFailed OnConnectionFailed;

        /// <summary>
        /// The client that we are using to communicate
        /// </summary>
        private UdpClient udpClient;

        /// <summary>
        /// The address we are connected to, if any
        /// </summary>
        private IPAddress address;

        /// <summary>
        /// The port we are connected to
        /// </summary>
        private int port;

        /// <summary>
        /// Are we connected to a server?
        /// </summary>
        private bool connected;

        /// <summary>
        /// Are we connecting to a server?
        /// </summary>
        private bool connecting;

        /// <summary>
        /// Are we running, or should we shutdown
        /// </summary>
        private volatile bool running = true;

        /// <summary>
        /// Attempt connection to a network address
        /// </summary>
        public void AttemptConnection(string address, int port, int timeout = 5000)
        {
            //Check if we are connected to a server
            if (connected || connecting)
            {
                throw new Exception("Cannot connect to server, we are already connected!");
            }
            //Format the IP address
            IPAddress ipAddress = IPAddress.Parse(address);
            //Mark us as attempting to connect
            connecting = true;
            Logger?.WriteLine($"Attempting connection to {address}:{port} (Timeout: {timeout}ms)", LogType.MESSAGE);
            //Attempt a connection
            udpClient = new UdpClient();
            udpClient.Connect(ipAddress, port);
            //Begin the timeout task
            Task.Run(() =>
            {
                float timeLeft = timeout;
                while (timeLeft > 0)
                {
                    //Check occassionally
                    Thread.Sleep(50);
                    //Reduce the time left
                    timeLeft -= 50;
                    //Check connected
                    if (udpClient.Client.Connected)
                    {
                        //Set the port and address
                        this.port = port;
                        this.address = ipAddress;
                        //Connection was successful, send connection packet
                        //Start the timeout task
                        Task.Run(() =>
                        {
                            //Sleep for timeout
                            Thread.Sleep(timeout);
                            //Check if connection valid
                            if (connected || udpClient == null)
                                return;
                            //Stop connecting
                            connecting = false;
                            //Disconnect
                            udpClient.Close();
                            //Timed out
                            OnConnectionFailed?.Invoke(ipAddress, DisconnectReason.TIMEOUT, "Connection timed out, server is not responding to connection requests.");
                        });
                        //Start the networking thread
                        Thread networkingThread = new Thread(() => NetworkingThread(udpClient));
                        networkingThread.Name = $"Networking thread ({address}:{port})";
                        networkingThread.Start();
                        //Log message
                        Logger?.WriteLine($"UDPClient connection established, sending connection request to server...", LogType.MESSAGE);
                        //Create connection packet
                        byte[] connectionPacket = BitConverter.GetBytes((int)PacketHeaders.CONNECTION_REQUEST);
                        //Send connection packet
                        udpClient.Send(connectionPacket, connectionPacket.Length);
                        return;
                    }
                }
                //Stop connecting
                connecting = false;
                //Timed out
                OnConnectionFailed?.Invoke(ipAddress, DisconnectReason.TIMEOUT, "Connection timed out, failed to connect to server.");
            });
        }

        /// <summary>
        /// The networking thread
        /// </summary>
        private void NetworkingThread(UdpClient client)
        {
            //Continue always
            while (running && (client.Client?.Connected ?? false))
            {
                try
                {
                    //Wait until we are woken up
                    IPEndPoint remoteEndPointer = new IPEndPoint(IPAddress.Any, port);
                    byte[] incomingData = udpClient.Receive(ref remoteEndPointer);
                    //Handle incomming data
                    Task.Run(() => HandleMessage(remoteEndPointer, incomingData));
                }
                catch (Exception e)
                {
                    Logger?.WriteLine(e, LogType.ERROR);
                }
            }
            //Disconnected
            Logger?.WriteLine("Disconnected from remote server.", LogType.MESSAGE);
        }

        /// <summary>
        /// Handle an incoming message
        /// </summary>
        private void HandleMessage(IPEndPoint sender, byte[] message)
        {
            //Ignore messages from people we weren't connecting to.
            //All communications must go through the server.
            //This is for security reasons, so a hacked client can't tell other players
            //invalid information.
            if (!sender.Address.Equals(address))
                return;
            //Process messages
            int packetHeader = BitConverter.ToInt32(message, 0);
            if (!connected)
            {
                //Switch the message
                switch ((PacketHeaders)packetHeader)
                {
                    case PacketHeaders.CONNECTION_ACCEPT:
                        //Conection accepted :)
                        connecting = false;
                        connected = true;
                        //Trigger the connection success event
                        OnConnectionSuccess?.Invoke(address);
                        Logger?.WriteLine($"Successfully connected to server {address}:{port}", LogType.MESSAGE);
                        return;
                    case PacketHeaders.CONNECTION_REJECT:
                        //Disconnect
                        connected = false;
                        connecting = false;
                        udpClient.Close();
                        udpClient = null;
                        //Get the message from the packet
                        string rejectionMessage = Encoding.UTF8.GetString(message.Skip(4).ToArray());
                        //Damn
                        Logger?.WriteLine($"Connection rejected by remote host. Reason: '{rejectionMessage}'", LogType.MESSAGE);
                        //Trigger the connection rejected event
                        OnConnectionFailed?.Invoke(address, DisconnectReason.CONNECTION_REJECTED, rejectionMessage);
                        return;
                }
            }
            else
            {
                //Handle connected packets
            }
        }

    }
}
