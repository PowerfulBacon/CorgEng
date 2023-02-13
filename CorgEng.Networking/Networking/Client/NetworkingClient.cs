using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Config;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.Networking.EntitySystems;
using CorgEng.Networking.Events;
using CorgEng.Networking.Networking.Server;
using CorgEng.Networking.VersionSync;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        [UsingDependency]
        private static IEntityCommunicator EntityCommunicator;

        [UsingDependency]
        private static IPrototypeManager PrototypeManager;

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

        /// <summary>
        /// If this is true when shutdownThreadTrigger is raised, task will immediately end.
        /// </summary>
        private bool cancelToken = false;

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
            if (OnConnectionFailed == null)
            {
                float closureAttempts = 0;
                OnConnectionFailed = (failureAddress, reason, message) => {
                    Logger.WriteLine($"Failed to connect to {failureAddress}.\n{reason}\n{message}", LogType.WARNING);
                    if (closureAttempts < 5)
                    {
                        Logger.WriteLine($"Attempting connection {closureAttempts}/5", LogType.WARNING);
                        closureAttempts++;
                        AttemptConnection(address, port, timeout);
                    }
                };
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
            ClientCommunicator.client = this;
            //Begin the timeout task
            Task.Run(() =>
            {
                try
                {
                    float timeLeft = timeout;
                    while (timeLeft > 0)
                    {
                        //Check occassionally
                        shutdownThreadTrigger.WaitOne(50);
                        if (cancelToken)
                            return;
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
                                shutdownThreadTrigger.WaitOne(timeout);
                                if (cancelToken)
                                    return;
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
                            shutdownCountdown.Reset();
                            Thread networkingThread = new Thread(() => NetworkListenerThread(udpClient));
                            networkingThread.Name = $"Client Listener thread ({address}:{port})";
                            networkingThread.Start();
                            //Start the packet queue thread
                            Thread packetProcessingThread = new Thread(ProcessQueueThread);
                            packetProcessingThread.Name = $"Packet processing thread ({port})";
                            packetProcessingThread.Start();
                            //Start the sender thread
                            Thread senderThread = new Thread(() => NetworkSenderThread(udpClient));
                            senderThread.Name = $"Client Sender thread ({address}:{port})";
                            senderThread.Start();
                            //Started
                            started = true;
                            //Log message
                            Logger?.WriteLine($"UDPClient connection established, sending connection request to server...", LogType.MESSAGE);
                            //Create connection packet
                            INetworkMessage networkMessage = NetworkMessageFactory.CreateMessage(
                                PacketHeaders.CONNECTION_REQUEST,
                                //Insert the networked ID into 0x00
                                BitConverter.GetBytes(VersionGenerator.NetworkVersion)
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
                    while (PacketQueue.AcquireLockIfHasMessages())
                    {
                        try
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
                        finally
                        {
                            PacketQueue.ReleaseLock();
                        }
                        //Logger.WriteLine($"Sent packets to the server", LogType.TEMP);
                    }
                    //Wait for variable time to maintain the tick rate
                    stopwatch.Stop();
                    double elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    double waitTime = inverseTickrate - elapsedMilliseconds;
                    if (waitTime > 0)
                    {
                        //Sleep the thread
                        shutdownThreadTrigger.WaitOne((int)waitTime);
                    }
                }
                catch (Exception e)
                {
                    Logger?.WriteLine(e, LogType.ERROR);
                }
            }
            Logger?.WriteLine($"Client sender thread terminated", LogType.WARNING);
            shutdownCountdown.Signal();
        }

        private ConcurrentQueue<(IPEndPoint, byte[])> packetQueue = new ConcurrentQueue<(IPEndPoint, byte[])>();

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
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
                    byte[] incomingData = udpClient.Receive(ref remoteEndPoint);
                    //Handle incomming data
                    packetQueue.Enqueue((remoteEndPoint, incomingData));
                }
                catch (Exception e)
                {
                    Logger?.WriteLine(e, LogType.ERROR);
                }
            }
            //Disconnected.
            Logger?.WriteLine("Disconnected from remote server.", LogType.WARNING);
            shutdownCountdown.Signal();
        }

        private void ProcessQueueThread()
        {
            Logger?.WriteLine($"Packet queue processor thread started.", LogType.MESSAGE);
            while (running)
            {
                //Nothing to process
                if (packetQueue.Count == 0)
                {
                    Thread.Yield();
                    continue;
                }
                //Stuff to do
                try
                {
                    //Recieve messages
                    (IPEndPoint, byte[]) packet;
                    if (packetQueue.TryDequeue(out packet))
                    {
                        ProcessPacket(packet.Item1, packet.Item2);
                    }
                }
                catch (Exception e)
                {
                    Logger?.WriteLine($"Critical exception on processing thread: {e}", LogType.ERROR);
                }
            }
            //Log shutdown
            Logger?.WriteLine($"Packet queue processor thread stopped.", LogType.MESSAGE);
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
                Logger.WriteMetric("packet_size", data.Length.ToString());
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
            Logger.WriteMetric("message_size", length.ToString());
            Logger.WriteMetric("message_header", header.ToString());
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
                //Logger?.WriteLine($"Receieved client message: {header} from {sender.Address}", LogType.LOG);
                //Handle connected packets
                NetworkMessageReceived?.Invoke(header, data, start, length);
                //Handle the event
                switch (header)
                {
                    case PacketHeaders.LOCAL_EVENT_RAISED:
                        {
                            using (MemoryStream stream = new MemoryStream(data))
                            {
                                stream.Seek(start, SeekOrigin.Begin);
                                using (BinaryReader reader = new BinaryReader(stream))
                                {
                                    //Serialisatino length
                                    int serialisationLength = reader.ReadInt32();
                                    //Networked identifier
                                    ushort networkedIdentifier = reader.ReadUInt16();
                                    //Read the target entity
                                    uint entityIdentifier = reader.ReadUInt32();
                                    IEntity entityTarget = EntityManager.GetEntity(entityIdentifier);
                                    //Get the event that was raised
                                    INetworkedEvent raisedEvent = VersionGenerator.CreateTypeFromIdentifier<INetworkedEvent>(networkedIdentifier);
                                    //Deserialize the event
                                    raisedEvent.Deserialise(reader);
                                    if (entityTarget == null)
                                    {
                                        Logger.WriteMetric("delayed_event_target", entityIdentifier.ToString());
                                        //Queue the event to fire when the entity is created
                                        DelayedEventSystem.AddDelayedEvent(entityIdentifier, (entityTarget) => {
                                            Logger.WriteMetric("networked_local_event_delayed", raisedEvent.GetType().ToString());
                                            raisedEvent.Raise(entityTarget);
                                        });
                                        return;
                                    }
                                    Logger.WriteMetric("networked_local_event", raisedEvent.GetType().ToString());
                                    raisedEvent.Raise(entityTarget);
                                }
                            }
                            return;
                        }
                    case PacketHeaders.GLOBAL_EVENT_RAISED:
                        {
                            using (MemoryStream stream = new MemoryStream(data))
                            {
                                stream.Seek(start, SeekOrigin.Begin);
                                using (BinaryReader reader = new BinaryReader(stream))
                                {
                                    //Serialisatino length
                                    int serialisationLength = reader.ReadInt32();
                                    //Networked identifier
                                    ushort networkedIdentifier = reader.ReadUInt16();
                                    //Get the event that was raised
                                    INetworkedEvent raisedEvent = VersionGenerator.CreateTypeFromIdentifier<INetworkedEvent>(networkedIdentifier);
                                    Logger.WriteMetric("networked_global_event", raisedEvent.GetType().ToString());
                                    //Deserialize the event
                                    raisedEvent.Deserialise(reader);
                                    raisedEvent.RaiseGlobally(false);
                                }
                            }
                            return;
                        }
                    case PacketHeaders.PROTOTYPE_INFO:
                        PrototypeManager.GetPrototype(data.Skip(start).Take(length).ToArray());
                        return;
                    case PacketHeaders.ENTITY_DATA:
                        Task.Run(async () =>
                        {
                            IEntity createdEntity = await EntityCommunicator.DeserialiseEntity(data.Skip(start).Take(length).ToArray());
                            EntityManager.RegisterEntity(createdEntity);
                        });
                        return;
                    case PacketHeaders.UPDATE_CLIENT_VIEW:
                        using (MemoryStream memoryStream = new MemoryStream(data))
                        {
                            //Seek to the correct location
                            memoryStream.Seek(start, SeekOrigin.Begin);
                            using (BinaryReader reader = new BinaryReader(memoryStream))
                            {
                                float viewX = reader.ReadSingle();
                                float viewY = reader.ReadSingle();
                                float viewZ = reader.ReadSingle();
                                double viewOffsetX = reader.ReadDouble();
                                double viewOffsetY = reader.ReadDouble();
                                double viewOffsetWidth = reader.ReadDouble();
                                double viewOffsetHeight = reader.ReadDouble();
                                //Update our view
                                //Send a signal
                                new ModifyIsometricView(viewX + viewOffsetX, viewY + viewOffsetY, viewZ, viewOffsetWidth, viewOffsetHeight).RaiseGlobally();
                            }
                        }
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
            cancelToken = true;
            Logger?.WriteLine("Client cleanup called", LogType.LOG);
            running = false;
            shutdownThreadTrigger.Set();
            udpClient?.Close();
            udpClient?.Dispose();
            udpClient = null;
            connected = false;
            connecting = false;
            NetworkMessageReceived = null;
            //OnConnectionFailed = null;
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
            cancelToken = false;
            Logger?.WriteLine("Client cleanup completed!", LogType.LOG);
        }
    }
}
