using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.Networking.EntitySystems;
using CorgEng.Networking.VersionSync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Tests.NetworkingTests
{
    [TestClass]
    public class NetworkingTest
    {

        [UsingDependency]
        private static INetworkingServer Server;

        [UsingDependency]
        private static INetworkingClient Client;

        [UsingDependency]
        private static INetworkMessageFactory MessageFactory;

        [UsingDependency]
        private static ILogger Logger;

        [TestCleanup]
        public void AfterTest()
        {
            Server.Cleanup();
            Client.Cleanup();
            Logger?.WriteLine("TEST COMPLETED", LogType.DEBUG);
        }

        [TestMethod]
        [Timeout(3000)]
        public void TestNetworkConnection()
        {
            bool success = false;
            Server.StartHosting(5000);
            Client.OnConnectionSuccess += (IPAddress ipAddress) => { success = true;  };
            Client.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Fail("Connection failed, server rejected connection."); };
            Client.AttemptConnection("127.0.0.1", 5000, 1000);

            while (!success)
                Thread.Sleep(0);

        }

        [TestMethod]
        [Timeout(3000)]
        public void TestSendingToServer()
        {
            bool connected = false;
            bool success = false;
            Server.StartHosting(5001);
            Client.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Inconclusive("Connection failed, server rejected connection."); };
            Client.OnConnectionSuccess += (IPAddress ipAddress) => { connected = true; };

            Server.NetworkMessageReceived += (PacketHeaders packetHeader, byte[] message, int start, int end) =>
            {
                if (packetHeader == PacketHeaders.NETWORKING_TEST)
                {
                    string gotMessage = Encoding.ASCII.GetString(message.Skip(start).Take(end).ToArray());
                    if (gotMessage != "CORRECT")
                        Assert.Fail($"Recieved message did not contain correct content, content recieved: '{gotMessage}'");
                    success = true;
                }
            };

            Client.AttemptConnection("127.0.0.1", 5001, 1000);

            //Await connection to the server
            while (!connected)
                Thread.Sleep(0);

            //Send a server message to the client
            Client.QueueMessage(
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
            Server.StartHosting(5002);
            Client.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Inconclusive("Connection failed, server rejected connection."); };
            Client.OnConnectionSuccess += (IPAddress ipAddress) => { connected = true; };
            Client.NetworkMessageReceived += (PacketHeaders packetHeader, byte[] message, int start, int end) =>
            {
                if (packetHeader == PacketHeaders.NETWORKING_TEST)
                {
                    string gotMessage = Encoding.ASCII.GetString(message.Skip(start).Take(end).ToArray());
                    if (gotMessage != "CORRECT")
                        Assert.Fail($"Recieved message did not contain correct content, content recieved: '{gotMessage}'");
                    success = true;
                }
            };
            Client.AttemptConnection("127.0.0.1", 5002, 1000);

            //Await connection to the server
            while (!connected)
                Thread.Sleep(0);

            //Send a server message to the client
            Server.QueueMessage(
                Server.ClientAddressingTable.GetEveryone(),
                MessageFactory.CreateMessage(PacketHeaders.NETWORKING_TEST, Encoding.ASCII.GetBytes("CORRECT"))
                );

            while (!success)
                Thread.Sleep(0);
        }

        [TestMethod]
        public void TestClientKick()
        {
            Assert.Inconclusive("Test isn't implemented");
        }

        [TestMethod]
        public void TestBanning()
        {
            Assert.Inconclusive("Test isn't implemented");
        }

        private class NetworkedTestEvent : INetworkedEvent
        {

            public bool CanBeRaisedByClient => true;

            public int testNumber { get; set; }

            public void Deserialize(byte[] data)
            {
                testNumber = BitConverter.ToInt32(data, 0);
            }

            public byte[] Serialize()
            {
                return BitConverter.GetBytes(testNumber);
            }
        }

        private class NetworkedTestComponent : Component
        {
            public override bool SetProperty(string name, IPropertyDef property)
            {
                return false;
            }
        }

        private class NetworkedTestEntitySystem : EntitySystem
        {

            public int signalRecievedCount = 0;

            public override EntitySystemFlags SystemFlags { get; } = EntitySystemFlags.HOST_SYSTEM | EntitySystemFlags.CLIENT_SYSTEM;

            public override void SystemSetup()
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
            //Set up a test entity system
            NetworkedTestEntitySystem networkedTestEntitySystem = new NetworkedTestEntitySystem();
            networkedTestEntitySystem.SystemSetup();

            //Start a networking system
            NetworkSystem networkSystem = new NetworkSystem();
            networkSystem.SystemSetup();

            //Connect to the server
            bool success = false;
            Server.StartHosting(5003);
            Client.OnConnectionSuccess += (IPAddress ipAddress) => { success = true; };
            Client.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Fail("Connection failed, server rejected connection."); };
            Client.AttemptConnection("127.0.0.1", 5003, 1000);

            while (!success)
                Thread.Sleep(0);

            //Raise a global event
            NetworkedTestEvent testEvent = new NetworkedTestEvent();
            testEvent.testNumber = 142;
            testEvent.RaiseGlobally();

            Logger?.WriteLine($"Test event raised globally, ID: {testEvent.GetNetworkedIdentifier()}", LogType.DEBUG);

            while (networkedTestEntitySystem.signalRecievedCount != 2)
                Thread.Sleep(0);
        }

    }
}
