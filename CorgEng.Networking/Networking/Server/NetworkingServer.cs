using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.GenericInterfaces.Networking.Packets.PacketQueues;
using CorgEng.Networking.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// Set the server transmission tick rate
        /// </summary>
        public int TickRate { get; set; } = 32;

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
                    while (PacketQueue.HasMessages())
                    {
                        //Get the queued packet
                        IQueuedPacket queuedPacket = PacketQueue.DequeuePacket();

                        //Get a list of all clients we want to send to
                        foreach (IClient target in queuedPacket.Targets.GetClients())
                        {
                            target.SendMessage(udpClient, queuedPacket.Data, queuedPacket.TopPointer);
                        }
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
            Logger?.WriteLine($"Server sender thread terminated", LogType.ERROR);
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
                    Logger?.WriteLine($"Message of size {incomingData.Length} recieved from client.", LogType.TEMP);
                }
                catch (Exception e)
                {
                    Logger?.WriteLine($"Critical exception on server: {e}", LogType.ERROR);
                }
            }
            //Log shutdown
            Logger?.WriteLine($"Networking server shut down.", LogType.MESSAGE);
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
                    NetworkMessageReceived?.Invoke(header, data, start, length);
                }
                else
                {
                    //Switch the message
                    switch (header)
                    {
                        case PacketHeaders.CONNECTION_REQUEST:
                            HandleConnectionRequest(sender);
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                Logger?.WriteLine(e, LogType.ERROR);
            }
        }

        private void HandleConnectionRequest(IPEndPoint sender)
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
            //Just accept it for now
            Logger?.WriteLine($"Accepting connection from {sender}", LogType.DEBUG);
            //Create a client
            IClient createdClient = ClientFactory.CreateClient("default", sender);
            connectedClients.Add(sender.Address, createdClient);
            //Create connection packet
            QueueMessage(
                ClientAddressingTable.AddClient(createdClient),
                NetworkMessageFactory.CreateMessage(PacketHeaders.CONNECTION_ACCEPT, new byte[0]));
            //Send a connection event globally
            new ClientConnectedEvent(createdClient).RaiseGlobally();
        }

        public void QueueMessage(IClientAddress targets, INetworkMessage message)
        {
            PacketQueue.QueueMessage(targets, message);
        }

        public void Cleanup()
        {
            Logger?.WriteLine("Server cleanup called", LogType.LOG);
            running = false;
            udpClient?.Close();
            udpClient?.Dispose();
            udpClient = null;
            PacketQueue = null;
            ClientAddressingTable = null;
            connectedClients = new Dictionary<IPAddress, IClient>();
            NetworkMessageReceived = null;
        }
    }
}
