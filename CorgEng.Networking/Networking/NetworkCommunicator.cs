using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
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
using CorgEng.Networking.Components;
using CorgEng.Networking.EntitySystems;
using CorgEng.Networking.Events;
using CorgEng.Networking.Networking.Client;
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

namespace CorgEng.Networking.Networking
{
    internal abstract class NetworkCommunicator
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

        [UsingDependency]
        private static IClientAddressingTableFactory ClientAddressingTableFactory;

        [UsingDependency]
        private static IEntityFactory EntityFactory;

        public IClientAddressingTable ClientAddressingTable { get; private set; }

        protected IPacketQueue PacketQueue;

        public event NetworkMessageRecieved NetworkMessageReceived;

        /// <summary>
        /// The client that we are using to communicate
        /// </summary>
        protected UdpClient udpClient;

        /// <summary>
        /// The address we are connected to, if any
        /// </summary>
        protected IPAddress address;

        /// <summary>
        /// The port we are connected to
        /// </summary>
        protected int port;

        /// <summary>
        /// Are we connected to a server?
        /// </summary>
        protected bool connected;

        /// <summary>
        /// Are we connecting to a server?
        /// </summary>
        protected bool connecting;

        /// <summary>
        /// Are we running, or should we shutdown
        /// </summary>
        protected volatile bool running = false;

        /// <summary>
        /// The trigger that gets activated when shutdown is triggered
        /// </summary>
        protected readonly AutoResetEvent shutdownThreadTrigger = new AutoResetEvent(false);

        /// <summary>
        /// The countdown event that allows the shutdown method to wait until threads are shutdown.
        /// </summary>
        protected readonly CountdownEvent shutdownCountdown = new CountdownEvent(2);

        /// <summary>
        /// Have we ever been started?
        /// </summary>
        protected bool started = false;

        /// <summary>
        /// If this is true when shutdownThreadTrigger is raised, task will immediately end.
        /// </summary>
        protected bool cancelToken = false;

        public int TickRate { get; set; } = 32;

#if DEBUG
        private Random random = new Random();
#endif

        /// <summary>
        /// The sender thread.
        /// Runs when it needs to, transmits data to the server
        /// with a set tick rate.
        /// </summary>
        protected void NetworkSenderThread(UdpClient client)
        {
            Logger?.WriteLine($"{GetType().Name} sender for {address} thread successfull started.", LogType.DEBUG);
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
            Logger?.WriteLine($"{GetType().Name} sender thread terminated", LogType.WARNING);
            shutdownCountdown.Signal();
        }

        private ConcurrentQueue<(IPEndPoint, byte[])> packetQueue = new ConcurrentQueue<(IPEndPoint, byte[])>();

        /// <summary>
        /// The networking thread
        /// </summary>
        protected void NetworkListenerThread(UdpClient client)
        {
            Logger?.WriteLine($"{GetType().Name} listener for {address} thread successfull started.", LogType.DEBUG);
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

        protected void ProcessQueueThread()
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
#if DEBUG
                // Simulated packet-loss
                if (NetworkConfig.PacketDropProbability > 0 && random.Next() < NetworkConfig.PacketDropProbability)
                {
                    return;
                }
#endif
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
                    // Invoke received message
                    NetworkMessageReceived?.Invoke(packetHeader, data, originalPoint + 0x06, packetSize);
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
        protected abstract void HandleMessage(IPEndPoint sender, PacketHeaders header, byte[] data, int start, int length);

        public virtual void Cleanup()
        {
            cancelToken = true;
            Logger?.WriteLine($"{GetType().Name} cleanup called", LogType.LOG);
            running = false;
            shutdownThreadTrigger.Set();
            udpClient?.Close();
            udpClient?.Dispose();
            udpClient = null;
            connected = false;
            connecting = false;
            NetworkMessageReceived = null;
            NetworkConfig.ProcessClientSystems = false;
            if (!NetworkConfig.ProcessServerSystems)
                NetworkConfig.NetworkingActive = false;
            Logger?.WriteLine($"Waiting for {GetType().Name} cleanup completion...", LogType.LOG);
            //Wait for the threads to be closed
            if (started)
                shutdownCountdown.Wait();
            //Reset
            shutdownThreadTrigger.Reset();
            started = false;
            cancelToken = false;
            Logger?.WriteLine($"{GetType().Name} cleanup completed!", LogType.LOG);
        }

    }
}
