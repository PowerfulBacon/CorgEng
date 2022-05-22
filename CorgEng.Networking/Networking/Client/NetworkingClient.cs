using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Config;
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
        private static INetworkMessageFactory NetworkMessageFactory;

        [UsingDependency]
        private static IPacketQueueFactory PacketQueueFactory;

        [UsingDependency]
        private static INetworkConfig NetworkConfig;

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
        private volatile bool running = false;

        /// <summary>
        /// The trigger that gets activated when shutdown is triggered
        /// </summary>
        private readonly AutoResetEvent shutdownThreadTrigger = new AutoResetEvent(false);

        /// <summary>
        /// The countdown event that allows the shutdown method to wait until threads are shutdown.
        /// </summary>
        private readonly CountdownEvent shutdownCountdown = new CountdownEvent(2);

        /// <summary>
        /// Have we ever been started?
        /// </summary>
        private bool started = false;

        public int TickRate { get; set; } = 32;

        /// <summary>
        /// Attempt connection to a network address
        /// </summary>
        public void AttemptConnection(string address, int port, int timeout = 5000)
        {
            //Init packet queue
            if (PacketQueue == null)
            {
                PacketQueue = PacketQueueFactory.CreatePacketQueue();
            }
            //Check if we are connected to a server
            if (connected || connecting)
            {
                throw new Exception("Cannot connect to server, we are already connected!");
            }
            //Enable networking
            NetworkConfig.NetworkingActive = true;
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
                try
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
                            //Add the server's address to the addressing table
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
                                //Cleanup
                                Cleanup();
                                //Log message
                                Logger?.WriteLine($"Connection to {address}:{port} timed out due to server not responding to our messages (Server is online but ignoring us).", LogType.LOG);
                                //Timed out
                                OnConnectionFailed?.Invoke(ipAddress, DisconnectReason.TIMEOUT, "Connection timed out, server is not responding to connection requests.");
                            });
                            //Start the networking thread
                            running = true;
                            Thread networkingThread = new Thread(() => NetworkListenerThread(udpClient));
                            networkingThread.Name = $"Client Listener thread ({address}:{port})";
                            networkingThread.Start();
                            //Start the sender thread
                            Thread senderThread = new Thread(() => NetworkSenderThread(udpClient));
                            senderThread.Name = $"Client Sender thread ({address}:{port})";
                            senderThread.Start();
                            //Started
                            started = true;
                            shutdownCountdown.Reset();
                            //Log message
                            Logger?.WriteLine($"UDPClient connection established, sending connection request to server...", LogType.MESSAGE);
                            //Create connection packet
                            INetworkMessage networkMessage = NetworkMessageFactory.CreateMessage(
                                PacketHeaders.CONNECTION_REQUEST,
                                //Insert the networked ID into 0x00
                                BitConverter.GetBytes(EventNetworkExtensions.NetworkedID)
                                );
                            //Send connection packet
                            QueueMessage(networkMessage);
                            return;
                        }
                    }
                    //Stop connecting
                    connecting = false;
                    //Time out
                    Logger?.WriteLine($"Connection to {address}:{port} timed out due to failing to connect to server (Server is offline).", LogType.LOG);
                    //Timed out
                    OnConnectionFailed?.Invoke(ipAddress, DisconnectReason.TIMEOUT, "Connection timed out, failed to connect to server.");
                }
                catch (Exception e)
                {
                    Logger?.WriteLine(e, LogType.ERROR);
                }
            });
        }

        /// <summary>
        /// Send a networking message to the server
        /// </summary>
        public void QueueMessage(INetworkMessage message)
        {
            PacketQueue?.QueueMessage(null, message);
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
                        byte[] data = queuedPacket.Data;
                        //Asynchronously send the data
                        //We send all the data straight to the server.
                        //The client cannot communicate with other clients.
                        udpClient.SendAsync(data, queuedPacket.TopPointer);
                    }
                    //Wait for variable time to maintain the tick rate
                    stopwatch.Stop();
                    double elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    double waitTime = inverseTickrate - elapsedMilliseconds;
                    //Sleep the thread
                    shutdownThreadTrigger.WaitOne((int)waitTime);
                }
                catch (Exception e)
                {
                    Logger?.WriteLine(e, LogType.ERROR);
                }
            }
            Logger?.WriteLine($"Client sender thread terminated", LogType.ERROR);
            shutdownCountdown.Signal();
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
            shutdownCountdown.Signal();
        }

        private void ProcessPacket(IPEndPoint sender, byte[] data)
        {
            try
            {
                //Ignore messages from people we weren't connecting to.
                //All communications must go through the server.
                //This is for security reasons, so a hacked client can't tell other players
                //invalid information.
                if (!sender.Address.Equals(address))
                {
                    return;
                }
                //Convert the packet into the individual messages
                int messagePointer = 0;
                while (messagePointer < data.Length)
                {
                    int originalPoint = messagePointer;
                    //Read the integer (First 4 bytes is the size of the message)
                    int packetSize = BitConverter.ToInt16(data, originalPoint);
                    //Move the message pointer along
                    messagePointer += packetSize + 0x06;
                    //Read the packet header
                    PacketHeaders packetHeader = (PacketHeaders)BitConverter.ToInt32(data, originalPoint + 0x02);
                    //Get the data and pass it on
                    HandleMessage(sender, packetHeader, data, originalPoint + 0x06, packetSize);
                    
                }
            }
            catch (Exception e)
            {
                Logger?.WriteLine(e, LogType.ERROR);
            }
        }

        /// <summary>
        /// Handle an incoming message
        /// </summary>
        private void HandleMessage(IPEndPoint sender, PacketHeaders header, byte[] data, int start, int length)
        {
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
                        NetworkConfig.ProcessClientSystems = true;
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
#if DEBUG
                    default:
                        Logger?.WriteLine($"Unknown packet header: {header}. This packet may be a bug or from a malicious attack (Debug build is on, so this message is shown which may slow the server down).", LogType.WARNING);
                        return;
#endif
                }
            }
            else
            {
                //Handle connected packets
                NetworkMessageReceived?.Invoke(header, data, start, length);
                //Handle the event
                switch (header)
                {
                   case PacketHeaders.GLOBAL_EVENT_RAISED:
                        //First we need to figure out what event is being raised
                        //Now we need to deserialize the byte data into the actual packet data
                        //Since implementation of this is specific to the classes, we need to create
                        //the correct class.
                        ushort eventID = BitConverter.ToUInt16(data, start);
                        //Get the event that was raised
                        Event raisedEvent = EventNetworkExtensions.GetEventFromNetworkedID(eventID);
                        //Deserialize the event
                        raisedEvent.Deserialize(data.Skip(start + 0x02).Take(length).ToArray());
                        raisedEvent.RaiseGlobally();
                        return;
#if DEBUG
                    default:
                        Logger?.WriteLine($"Unknown packet header: {header}. This packet may be a bug or from a malicious attack (Debug build is on, so this message is shown which may slow the server down).", LogType.WARNING);
                        return;
#endif
                }
            }
        }

        public void Cleanup()
        {
            Logger?.WriteLine("Client cleanup called", LogType.LOG);
            running = false;
            shutdownThreadTrigger.Set();
            udpClient?.Close();
            udpClient?.Dispose();
            udpClient = null;
            connected = false;
            connecting = false;
            NetworkMessageReceived = null;
            OnConnectionFailed = null;
            OnConnectionSuccess = null;
            NetworkConfig.ProcessClientSystems = false;
            if (!NetworkConfig.ProcessServerSystems)
                NetworkConfig.NetworkingActive = false;
            Logger?.WriteLine("Waiting for client cleanup completion...", LogType.LOG);
            //Wait for the threads to be closed
            if (started)
                shutdownCountdown.Wait();
            //Reset
            shutdownThreadTrigger.Reset();
            started = false;
            Logger?.WriteLine("Client cleanup completed!", LogType.LOG);
        }
    }
}
