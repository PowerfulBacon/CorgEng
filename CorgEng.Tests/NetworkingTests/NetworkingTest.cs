using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Injection;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.Networking.EntitySystems;
using CorgEng.Networking.Networking.Client;
using CorgEng.Networking.Networking.Server;
using CorgEng.Networking.VersionSync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Tests.NetworkingTests
{
    [TestClass]
    [DoNotParallelize]
    public class NetworkingTest
    {

        [UsingDependency]
        private static INetworkMessageFactory MessageFactory;

        [UsingDependency]
        private static IWorldFactory WorldFactory;

        [UsingDependency]
        private static ILogger Logger;

        [TestMethod]
        [Timeout(5000)]
        public void TestNetworkConnection()
        {
            bool success = false;

            IWorld clientWorld = WorldFactory.CreateWorld();
            IWorld serverWorld = WorldFactory.CreateWorld();

            serverWorld.ServerInstance.StartHosting(5000);
            clientWorld.ClientInstance.OnConnectionSuccess += (IPAddress ipAddress) => { success = true;  };
            clientWorld.ClientInstance.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Fail("Connection failed, server rejected connection."); };
            clientWorld.ClientInstance.AttemptConnection("127.0.0.1", 5000, 1000);

            while (!success)
                Thread.Sleep(0);

        }

        [TestMethod]
        [Timeout(3000)]
        public void TestSendingToServer()
        {
            bool connected = false;
            bool success = false;

            IWorld clientWorld = WorldFactory.CreateWorld();
            IWorld serverWorld = WorldFactory.CreateWorld();

            serverWorld.ServerInstance.StartHosting(5001);
            clientWorld.ClientInstance.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Inconclusive("Connection failed, server rejected connection."); };
            clientWorld.ClientInstance.OnConnectionSuccess += (IPAddress ipAddress) => { connected = true; };

            serverWorld.ServerInstance.NetworkMessageReceived += (PacketHeaders packetHeader, byte[] message, int start, int end) =>
            {
                if (packetHeader == PacketHeaders.NETWORKING_TEST)
                {
                    string gotMessage = Encoding.ASCII.GetString(message.Skip(start).Take(end).ToArray());
                    if (gotMessage != "CORRECT")
                        Assert.Fail($"Recieved message did not contain correct content, content recieved: '{gotMessage}'");
                    success = true;
                }
            };

            clientWorld.ClientInstance.AttemptConnection("127.0.0.1", 5001, 1000);

            //Await connection to the server
            while (!connected)
                Thread.Sleep(0);

            //Send a server message to the client
            clientWorld.ClientInstance.QueueMessage(
                MessageFactory.CreateMessage(PacketHeaders.NETWORKING_TEST, Encoding.ASCII.GetBytes("CORRECT"))
                );

            while (!success)
                Thread.Sleep(0);

        }

        [TestMethod]
        [Timeout(3000)]
        public void TestSendingToClient()
        {
            bool connected = false;
            bool success = false;

            IWorld clientWorld = WorldFactory.CreateWorld();
            IWorld serverWorld = WorldFactory.CreateWorld();

            serverWorld.ServerInstance.StartHosting(5002);
            clientWorld.ClientInstance.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Inconclusive("Connection failed, server rejected connection."); };
            clientWorld.ClientInstance.OnConnectionSuccess += (IPAddress ipAddress) => { connected = true; };
            clientWorld.ClientInstance.NetworkMessageReceived += (PacketHeaders packetHeader, byte[] message, int start, int end) =>
            {
                if (packetHeader == PacketHeaders.NETWORKING_TEST)
                {
                    string gotMessage = Encoding.ASCII.GetString(message.Skip(start).Take(end).ToArray());
                    if (gotMessage != "CORRECT")
                        Assert.Fail($"Recieved message did not contain correct content, content recieved: '{gotMessage}'");
                    success = true;
                }
            };
            clientWorld.ClientInstance.AttemptConnection("127.0.0.1", 5002, 1000);

            //Await connection to the server
            while (!connected)
                Thread.Sleep(0);

            //Send a server message to the client
            serverWorld.ServerInstance.QueueMessage(
                serverWorld.ServerInstance.ClientAddressingTable.GetEveryone(),
                MessageFactory.CreateMessage(PacketHeaders.NETWORKING_TEST, Encoding.ASCII.GetBytes("CORRECT"))
                );

            while (!success)
                Thread.Sleep(0);
        }

        [TestMethod]
        [Timeout(5000)]
        public void TestClientKick()
        {
            Assert.Inconclusive("Test isn't implemented");
        }

        [TestMethod]
        [Timeout(5000)]
        public void TestBanning()
        {
            Assert.Inconclusive("Test isn't implemented");
        }

        private class NetworkedTestEvent : INetworkedEvent
        {

            public bool CanBeRaisedByClient => true;

            public int testNumber { get; set; }

            public void Deserialise(BinaryReader reader)
            {
                testNumber = reader.ReadInt32();
            }

            public void Serialise(BinaryWriter writer)
            {
                writer.Write(testNumber);
            }

            public int SerialisedLength()
            {
                return sizeof(int);
            }

        }

        private class NetworkedTestComponent : Component
        { }

        private class NetworkedTestEntitySystem : EntitySystem
        {

            public int signalRecievedCount = 0;

            public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM | EntitySystemFlags.CLIENT_SYSTEM;

            public override void SystemSetup(IWorld world)
            {
                RegisterGlobalEvent<NetworkedTestEvent>(AcceptTestEvent);
            }

            private void AcceptTestEvent(NetworkedTestEvent networkedTestEvent)
            {
                Logger?.WriteLine("System recieved an event!", LogType.DEBUG);
                if (networkedTestEvent.testNumber != 142)
                    throw new Exception("Invalid networked test event number");
                signalRecievedCount++;
            }

        }

        /// <summary>
        /// Since we are running the client and host on the same instance,
        /// events will be duplicated. We need to make sure we recieve 2 events.
        /// </summary>
        [TestMethod]
        [Timeout(3000)]
        public void TestGlobalNetworkedEvent()
        {
            IWorld clientWorld = WorldFactory.CreateWorld();
            IWorld serverWorld = WorldFactory.CreateWorld();

            //Set up a test entity system
            NetworkedTestEntitySystem networkedTestEntitySystem = serverWorld.EntitySystemManager.GetSingleton<NetworkedTestEntitySystem>();
            //Start a networking system
            NetworkSystem networkSystem = new NetworkSystem();
            networkSystem.SystemSetup(clientWorld);

            //Connect to the server
            bool success = false;
            serverWorld.ServerInstance.StartHosting(5003);
            clientWorld.ClientInstance.OnConnectionSuccess += (IPAddress ipAddress) => { success = true; };
            clientWorld.ClientInstance.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Fail("Connection failed, server rejected connection."); };
            clientWorld.ClientInstance.AttemptConnection("127.0.0.1", 5003, 1000);

            while (!success)
                Thread.Sleep(0);

            //Raise a global event
            NetworkedTestEvent testEvent = new NetworkedTestEvent();
            testEvent.testNumber = 142;
            testEvent.RaiseGlobally(serverWorld);

            Logger?.WriteLine($"Test event raised globally, ID: {testEvent.GetNetworkedIdentifier()}", LogType.DEBUG);

            while (networkedTestEntitySystem.signalRecievedCount != 2)
                Thread.Sleep(0);
        }

    }
}
