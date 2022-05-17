using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [UsingDependency]
        private INetworkMessageFactory NetworkMessageFactory;

        [UsingDependency]
        private IPacketQueueFactory PacketQueueFactory;

        private IPacketQueue PacketQueue;

        public event ConnectionSuccess OnConnectionSuccess;

        public event ConnectionFailed OnConnectionFailed;

        public event NetworkMessageRecieved NetworkMessageReceived;

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

        public int TickRate { get; set; } = 32;

        public NetworkingClient()
        {
            PacketQueue = PacketQueueFactory.CreatePacketQueue();
        }

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
                        Thread networkingThread = new Thread(() => NetworkListenerThread(udpClient));
                        networkingThread.Name = $"Networking thread ({address}:{port})";
                        networkingThread.Start();
                        //Log message
                        Logger?.WriteLine($"UDPClient connection established, sending connection request to server...", LogType.MESSAGE);
                        //Create connection packet
                        INetworkMessage networkMessage = NetworkMessageFactory.CreateMessage(PacketHeaders.CONNECTION_REQUEST, new byte[0]);
                        //Send connection packet
                        QueueMessage(networkMessage);
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
        /// Send a networking message to the server
        /// </summary>
        public void QueueMessage(IClientAddress target, INetworkMessage message)
        {
            PacketQueue?.QueueMessage(target, message);
        }

        /// <summary>
        /// The sender thread.
        /// Runs when it needs to, transmits data to the server
        /// with a set tick rate.
        /// </summary>
        private void NetworkSenderThread(UdpClient client)
        {
            Logger?.WriteLine($"Client sender for {address} thread successfull started.", LogType.DEBUG);
            Stopwatch stopwatch = new Stopwatch();
            double inverseTickrate = 1000.0 / TickRate;
            while (running && (client.Client?.Connected ?? false))
            {
                try
                {
                    //Create a stopwatch to get the current time
                    stopwatch.Restart();
                    //Transmit packets
                    while (PacketQueue.HasMessages())
                    {
                        //Dequeue the packet from the queue
                        IQueuedPacket queuedPacket = PacketQueue.DequeuePacket();
                        //Transmit the packet to the server
                        byte[] data = queuedPacket.GetData();
                        //Asynchronously send the data
                        udpClient.SendAsync(data, data.Length);
                    }
                    //Wait for variable time to maintain the tick rate
                    stopwatch.Stop();
                    double elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    double waitTime = inverseTickrate - elapsedMilliseconds;
                    //Sleep the thread
                    Thread.Sleep((int)waitTime);
                }
                catch (Exception e)
                {
                    Logger?.WriteLine(e, LogType.ERROR);
                }
            }
        }

        /// <summary>
        /// The networking thread
        /// </summary>
        private void NetworkListenerThread(UdpClient client)
        {
            Logger?.WriteLine($"Client listener for {address} thread successfull started.", LogType.DEBUG);
            //Continue always
            while (running && (client.Client?.Connected ?? false))
            {
                try
                {
                    //Wait until we are woken up
                    IPEndPoint remoteEndPointer = new IPEndPoint(IPAddress.Any, port);
                    byte[] incomingData = udpClient.Receive(ref remoteEndPointer);
                    //Handle incomming data
                    Task.Run(() => ProcessPacket(remoteEndPointer, incomingData));
                }
                catch (Exception e)
                {
                    Logger?.WriteLine(e, LogType.ERROR);
                }
            }
            //Disconnected
            Logger?.WriteLine("Disconnected from remote server.", LogType.MESSAGE);
        }

        private void ProcessPacket(IPEndPoint sender, byte[] data)
        {
            //Ignore messages from people we weren't connecting to.
            //All communications must go through the server.
            //This is for security reasons, so a hacked client can't tell other players
            //invalid information.
            if (!sender.Address.Equals(address))
                return;
            //Convert the packet into the individual messages
            int messagePointer = 0;
            while (messagePointer < data.Length)
            {
                //Read the integer (First 4 bytes is the size of the message)
                int packetSize = BitConverter.ToInt32(data, messagePointer);
                //Read the packet header
                PacketHeaders packetHeader = (PacketHeaders)BitConverter.ToInt32(data, messagePointer + 0x04);
                //Get the data and pass it on
                HandleMessage(sender, packetHeader, data, messagePointer + 0x08, packetSize - 0x08);
                //Move the message pointer along
                messagePointer += packetSize;
            }
        }

        /// <summary>
        /// Handle an incoming message
        /// </summary>
        private void HandleMessage(IPEndPoint sender, PacketHeaders header, byte[] data, int start, int length)
        {
            //Ignore messages from people we weren't connecting to.
            //All communications must go through the server.
            //This is for security reasons, so a hacked client can't tell other players
            //invalid information.
            if (!sender.Address.Equals(address))
                return;
            //Process messages
            if (!connected)
            {
                //Switch the message
                switch (header)
                {
                    case PacketHeaders.CONNECTION_ACCEPT:
                        //Conection accepted :)
                        connecting = false;
                        connected = true;
                        //Trigger the connection success event
                        OnConnectionSuccess?.Invoke(address);
                        //Start the network transmission thread
                        Thread networkTransmissionThread = new Thread(() => NetworkSenderThread(udpClient));
                        networkTransmissionThread.Name = $"Client transmission to {sender.Address}";
                        networkTransmissionThread.Start();
                        Logger?.WriteLine($"Successfully connected to server {address}:{port}", LogType.MESSAGE);
                        return;
                    case PacketHeaders.CONNECTION_REJECT:
                        //Disconnect
                        connected = false;
                        connecting = false;
                        udpClient.Close();
                        udpClient = null;
                        //Get the message from the packet
                        byte[] trimmedData = data.Skip(start).Take(length).ToArray();
                        string rejectionMessage = Encoding.UTF8.GetString(trimmedData);
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

        public void Cleanup()
        {
            throw new NotImplementedException();
        }
    }
}
