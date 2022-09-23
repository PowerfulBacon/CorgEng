using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Events.Events;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
using CorgEng.EntityComponentSystem.Implementations.Transform;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Config;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.Networking.Components;
using CorgEng.Networking.Events;
using CorgEng.Networking.VersionSync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        [UsingDependency]
        private static IClientFactory ClientFactory;

        [UsingDependency]
        private static IClientAddressingTableFactory ClientAddressingTableFactory;

        public IClientAddressingTable ClientAddressingTable { get; private set; }

        [UsingDependency]
        private static INetworkMessageFactory NetworkMessageFactory;

        [UsingDependency]
        private static IPacketQueueFactory PacketQueueFactory;

        [UsingDependency]
        private static INetworkConfig NetworkConfig;

        [UsingDependency]
        private static IPrototypeManager PrototypeManager;

        private static IPrototype DefaultEntityPrototype;

        private IPacketQueue PacketQueue;

        /// <summary>
        /// The client we are using to communicate
        /// </summary>
        private UdpClient udpClient;

        /// <summary>
        /// Mark false if the server should shut down
        /// </summary>
        private volatile bool running = false;

        /// <summary>
        /// The port the server is currently running on.
        /// </summary>
        private int port;

        /// <summary>
        /// A dictionary containing all connected clients
        /// </summary>
        private Dictionary<IPAddress, IClient> connectedClients = new Dictionary<IPAddress, IClient>();

        public event NetworkMessageRecieved NetworkMessageReceived;

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
        /// Set the server transmission tick rate
        /// </summary>
        public int TickRate { get; set; } = 32;

        [ModuleLoad]
        public void LoadDefaultPrototype()
        {
            //Set the default prototype
            IEntity sampleEntity = new Entity();
            sampleEntity.AddComponent(new NetworkTransformComponent());
            sampleEntity.AddComponent(new ClientComponent());
            //Get the prototype
            DefaultEntityPrototype = PrototypeManager.GetPrototype(sampleEntity, false);
            //Delete the entity
            new DeleteEntityEvent().Raise(sampleEntity);
        }

        public void StartHosting(int port)
        {
            if (ClientAddressingTable == default)
            {
                ClientAddressingTable = ClientAddressingTableFactory.CreateAddressingTable();
            }
            //Initialize the packet queue
            if (PacketQueue == default)
            {
                PacketQueue = PacketQueueFactory.CreatePacketQueue();
            }
            //We are already connected to the UDP client
            if (udpClient != null && udpClient.Client.Connected)
            {
                throw new Exception($"Server on port {port} is already running.");
            }
            //Start networking
            NetworkConfig.NetworkingActive = true;
            //Log
            Logger?.WriteLine($"Starting UDP server on port {port}.", LogType.MESSAGE);
            //Set the listening port
            this.port = port;
            //Start the server (Doesn't connect to anything, just listens)
            udpClient = new UdpClient(port);
            //Start running the serve
            running = true;
            //Start the networking thread
            Thread serverThread = new Thread(NetworkListenerThread);
            serverThread.Name = $"Networking Listener ({port})";
            serverThread.Start();
            //Start the transmission thread
            Thread transmissionThread = new Thread(NetworkSenderThread);
            transmissionThread.Name = $"Networking Transmitter ({port})";
            transmissionThread.Start();
            shutdownCountdown.Reset();
            started = true;
            ServerCommunicator.server = this;
            NetworkConfig.ProcessServerSystems = true;
        }

        /// <summary>
        /// The sender thread.
        /// Runs when it needs to, transmits data to the server
        /// with a set tick rate.
        /// </summary>
        private void NetworkSenderThread()
        {
            Logger?.WriteLine($"Server sender for port:{port} thread successfull started.", LogType.DEBUG);
            Stopwatch stopwatch = new Stopwatch();
            double inverseTickrate = 1000.0 / TickRate;
            while (running)
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
                            //Get the queued packet
                            IQueuedPacket queuedPacket = PacketQueue.DequeuePacket();

                            //Get a list of all clients we want to send to
                            foreach (IClient target in queuedPacket.Targets.GetClients())
                            {
                                target.SendMessage(udpClient, queuedPacket.Data, queuedPacket.TopPointer);
                            }
                        }
                        finally
                        {
                            PacketQueue.ReleaseLock();
                        }
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
            Logger?.WriteLine($"Server sender thread terminated", LogType.ERROR);
            shutdownCountdown.Signal();
        }

        private void NetworkListenerThread()
        {
            Logger?.WriteLine($"Server listening thread started on port {port}.", LogType.MESSAGE);
            while (running)
            {
                try
                {
                    //Recieve messages
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
                    byte[] incomingData = udpClient.Receive(ref remoteEndPoint);
                    Task.Run(() => ProcessPacket(remoteEndPoint, incomingData));
                }
                catch (Exception e)
                {
                    Logger?.WriteLine($"Critical exception on server: {e}", LogType.ERROR);
                }
            }
            //Log shutdown
            Logger?.WriteLine($"Networking server shut down.", LogType.MESSAGE);
            shutdownCountdown.Signal();
        }

        private void ProcessPacket(IPEndPoint sender, byte[] data)
        {
            try
            {
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

        private void HandleMessage(IPEndPoint sender, PacketHeaders header, byte[] data, int start, int length)
        {
            try
            {
                //Check the message header
                //Process messages
                if (connectedClients.ContainsKey(sender.Address))
                {
                    //The client this message is coming from
                    IClient client = connectedClients[sender.Address];
                    switch (header)
                    {
                        case PacketHeaders.REQUEST_PROTOTYPE:
                            //Get the prototype identifier
                            uint prototypeIdentifier = BitConverter.ToUInt32(data, start);
                            //Locate the prototype that is being requested
                            IPrototype prototypeRequested = PrototypeManager.GetLocalProtoype(prototypeIdentifier);
                            //We don't have that, ignore the request
                            if (prototypeRequested == null)
                                return;
                            //Send the prototype to the client
                            INetworkMessage message = NetworkMessageFactory.CreateMessage(
                                PacketHeaders.PROTOTYPE_INFO,
                                prototypeRequested.SerializePrototype()
                            );
                            QueueMessage(ClientAddressingTable.GetFlagRepresentation(client), message);
                            return;
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
                                        Logger.WriteLine($"global event raised of type {raisedEvent.GetType()}");
                                        //Deserialize the event
                                        raisedEvent.Deserialise(reader);
                                        raisedEvent.RaiseGlobally(false);
                                    }
                                }
                                return;
                            }
#if DEBUG
                        default:
                            Logger?.WriteLine($"Unhandled header: {header}", LogType.WARNING);
                            break;
#endif
                    }
                    NetworkMessageReceived?.Invoke(header, data, start, length);
                }
                else
                {
                    //Switch the message
                    switch (header)
                    {
                        case PacketHeaders.CONNECTION_REQUEST:
                            HandleConnectionRequest(sender, data, start, length);
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                Logger?.WriteLine(e, LogType.ERROR);
            }
        }

        private void HandleConnectionRequest(IPEndPoint sender, byte[] data, int start, int length)
        {
            //Refuse connection if already connected
            if (connectedClients.ContainsKey(sender.Address))
            {
                //Create rejection packet
                QueueMessage(
                    ClientAddressingTable.GetFlagRepresentation(connectedClients[sender.Address]),
                    NetworkMessageFactory.CreateMessage(PacketHeaders.CONNECTION_REJECT, Encoding.ASCII.GetBytes("Already connected to server.")));
                return;
            }
            //Check version identifier
            int clientVersionID = BitConverter.ToInt32(data, start);
            if (clientVersionID != VersionGenerator.NetworkVersion)
            {
                Logger?.WriteLine($"Incoming client has incorrect version ID: {clientVersionID}, expected: {VersionGenerator.NetworkVersion}");
                //Create rejection packet
                QueueMessage(
                    ClientAddressingTable.GetFlagRepresentation(connectedClients[sender.Address]),
                    NetworkMessageFactory.CreateMessage(PacketHeaders.CONNECTION_REJECT, Encoding.ASCII.GetBytes($"Networked version ID mismatch, {VersionGenerator.NetworkVersion} =/= {clientVersionID}")));
                return;
            }
            //Just accept it for now
            Logger?.WriteLine($"Accepting connection from {sender}", LogType.DEBUG);
            //Create a client
            IClient createdClient = ClientFactory.CreateClient("default", sender);
            connectedClients.Add(sender.Address, createdClient);
            //Create connection packet
            QueueMessage(
                ClientAddressingTable.AddClient(createdClient),
                NetworkMessageFactory.CreateMessage(PacketHeaders.CONNECTION_ACCEPT, new byte[0]));
            //Create a new client entity and add what we need
            IEntity createdEntity = DefaultEntityPrototype.CreateEntityFromPrototype();
            //Send a connection event
            new ClientConnectedEvent(createdClient).Raise(createdEntity);
            //Initialise the entity
            new InitialiseNetworkedEntityEvent().Raise(createdEntity);
        }

        public void QueueMessage(IClientAddress targets, INetworkMessage message)
        {
            PacketQueue.QueueMessage(targets, message);
        }

        public void Cleanup()
        {
            Logger?.WriteLine("Server cleanup called...", LogType.LOG);
            running = false;
            shutdownThreadTrigger.Set();
            udpClient?.Close();
            udpClient?.Dispose();
            udpClient = null;
            PacketQueue = null;
            ClientAddressingTable = null;
            connectedClients = new Dictionary<IPAddress, IClient>();
            NetworkMessageReceived = null;
            NetworkConfig.ProcessServerSystems = false;
            if (!NetworkConfig.ProcessClientSystems)
                NetworkConfig.NetworkingActive = false;
            if (ServerCommunicator.server == this)
                ServerCommunicator.server = null;
            Logger?.WriteLine("Waiting for server cleanup completion...", LogType.LOG);
            //Wait for the threads to be closed
            if(started)
                shutdownCountdown.Wait();
            //Reset
            shutdownThreadTrigger.Reset();
            started = false;
            Logger?.WriteLine("Server cleanup completed!", LogType.LOG);
        }

        public void SetClientPrototype(IPrototype prototype)
        {
            DefaultEntityPrototype = prototype ?? throw new ArgumentNullException(nameof(prototype));
        }

    }
}
