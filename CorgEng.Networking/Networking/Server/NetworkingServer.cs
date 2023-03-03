using CorgEng.Core;
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

namespace CorgEng.Networking.Networking.Server
{

    [Dependency]
    internal class NetworkingServer : NetworkCommunicator, INetworkingServer
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

        [UsingDependency]
        private static IEntityFactory EntityFactory;

        [UsingDependency]
        private static IQueuedPacketFactory QueuedPacketFactory = null!;

        private static IPrototype DefaultEntityPrototype;

        /// <summary>
        /// A dictionary containing all connected clients
        /// </summary>
        internal Dictionary<IPAddress, IClient> connectedClients = new Dictionary<IPAddress, IClient>();

        private volatile EventWaitHandle messageReadyWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        private volatile bool threadWaiting = false;

        private double lastPingAt = 0;

        [ModuleLoad]
        public void LoadDefaultPrototype()
        {
            //Set the default prototype
            EntityFactory.CreateEmptyEntity(sampleEntity => {
                sampleEntity.AddComponent(new NetworkTransformComponent());
                sampleEntity.AddComponent(new ClientComponent());
                //Get the prototype
                DefaultEntityPrototype = PrototypeManager.GetPrototype(sampleEntity, false);
                //Delete the entity
                new DeleteEntityEvent().Raise(sampleEntity, true);
            });
        }

        /// <summary>
        /// Start hosting a network server
        /// </summary>
        /// <param name="port"></param>
        /// <exception cref="Exception"></exception>
        public virtual void StartHosting(int port)
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
            Logger.WriteLine($"UDP Server has buffer size of {udpClient.Client.ReceiveBufferSize}", LogType.DEBUG);
            running = true;
            //Start the networking thread
            Thread serverThread = new Thread(() => NetworkListenerThread(udpClient));
            serverThread.Name = $"Networking Listener ({port})";
            serverThread.Start();
            //Start the packet queue thread
            Thread packetProcessingThread = new Thread(ProcessQueueThread);
            packetProcessingThread.Name = $"Packet processing thread ({port})";
            packetProcessingThread.Start();
            //Start the transmission thread
            Thread transmissionThread = new Thread(() => NetworkSenderThread(udpClient));
            transmissionThread.Name = $"Networking Transmitter ({port})";
            transmissionThread.Start();
            shutdownCountdown.Reset();
            started = true;
            ServerCommunicator.server = this;
            NetworkConfig.ProcessServerSystems = true;
        }

        protected override void HandleMessage(IPEndPoint sender, PacketHeaders header, byte[] data, int start, int length)
        {
            try
            {
                Logger.WriteMetric("message_size", length.ToString());
                Logger.WriteMetric("message_header", header.ToString());
                //Logger?.WriteLine($"Receieved server message: {header} from {sender.Address}", LogType.LOG);
                //Check the message header
                //Process messages
                if (connectedClients.TryGetValue(sender.Address, out IClient client))
                {
                    switch (header)
                    {
                        case PacketHeaders.REQUEST_PROTOTYPE:
                            //Get the prototype identifier
                            uint prototypeIdentifier = BitConverter.ToUInt32(data, start);
                            Logger.WriteMetric("networked_prototype_requested", prototypeIdentifier.ToString());
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
                                        Logger.WriteMetric("networked_global_event", raisedEvent.ToString());
                                        //Deserialize the event
                                        raisedEvent.Deserialise(reader);
                                        raisedEvent.RaiseGlobally(false);
                                    }
                                }
                                return;
                            }
                        case PacketHeaders.PING_RESPONSE:
                            {
                                double sentAt = BitConverter.ToDouble(data, start);
                                client.RoundTripPing = CorgEngMain.Time - sentAt;
                                Logger.WriteLine($"Recieved ping from {sender.Address}:{sender.Port} ({client.Username})\nSent at:\t\t{sentAt}s\nRecieved at:\t\t{CorgEngMain.Time}s\nRound trip time:\t{CorgEngMain.Time - sentAt}s", LogType.TEMP);
                                return;
                            }
                        case PacketHeaders.ACKNOWLEDGE_PACKET:
                            // Do nothing
                            return;
#if DEBUG
                        default:
                            Logger?.WriteLine($"Unhandled header: {header}", LogType.WARNING);
                            break;
#endif
                    }
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
                Logger?.WriteLine($"Incoming client has incorrect version ID: {clientVersionID}, expected: {VersionGenerator.NetworkVersion}", LogType.NETWORK_LOG);
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
            if (threadWaiting)
                messageReadyWaitHandle.Set();
        }

        public void SetClientPrototype(IPrototype prototype)
        {
            DefaultEntityPrototype = prototype ?? throw new ArgumentNullException(nameof(prototype));
        }

    }
}
