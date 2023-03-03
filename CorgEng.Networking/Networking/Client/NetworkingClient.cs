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
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
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
    /// <summary>
    /// The networking client.
    /// This is absolutely awful and I hate it, will probably rewrite it in the future
    /// to iron out some of the oversights.
    /// </summary>
    [Dependency]
    internal class NetworkingClient : NetworkCommunicator, INetworkingClient
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

        public event ConnectionSuccess OnConnectionSuccess;

        public event ConnectionFailed OnConnectionFailed;

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
        /// Handle an incoming message
        /// </summary>
        protected override void HandleMessage(IPEndPoint sender, PacketHeaders header, byte[] data, int start, int length)
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
                                            Logger.WriteMetric("networked_local_event_delayed", raisedEvent.ToString());
                                            raisedEvent.Raise(entityTarget);
                                        });
                                        // Request info about this entity
                                        //TODO
                                        return;
                                    }
                                    Logger.WriteMetric("networked_local_event", raisedEvent.ToString());
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
                    // Ping requests need to not include delay
                    case PacketHeaders.PING_REQUEST:
                        // Send back the packet immediately, without waiting for the server to reach the next net tick
                        // We trust the server to be correct, so don't ignore pings if they are being spammed unlike the server.
                        double sentAt = BitConverter.ToDouble(data, start);
                        INetworkMessage networkMessage = NetworkMessageFactory.CreateMessage(PacketHeaders.PING_RESPONSE, BitConverter.GetBytes(sentAt));
                        IQueuedPacket packet = QueuedPacketFactory.CreatePacket(null, networkMessage.GetBytes());
                        udpClient.Send(packet.Data, packet.TopPointer);
                        return;
                    case PacketHeaders.ACKNOWLEDGE_PACKET:
                        long packetIdentifier = BitConverter.ToInt64(data, start);
                        //TODO Allow for binary lists to contain long identifiers, 2^64 values instead of 2^31
                        //packetConfirmationQueue.Remove((int)packetIdentifier);
                        return;
#if DEBUG
                    default:
                        Logger?.WriteLine($"Unknown packet header: {header}. This packet may be a bug or from a malicious attack (Debug build is on, so this message is shown which may slow the server down).", LogType.WARNING);
                        return;
#endif
                }
            }
        }

        public override void Cleanup()
        {
            //OnConnectionFailed = null;
            OnConnectionSuccess = null;
            base.Cleanup();
        }

    }
}
