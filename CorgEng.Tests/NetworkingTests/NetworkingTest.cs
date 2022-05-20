using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.Packets;
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

        [TestCleanup]
        public void AfterTest()
        {
            Server.Cleanup();
            Client.Cleanup();
        }

        [TestMethod]
        [Timeout(1500)]
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
        [Timeout(1500)]
        public void TestSendingToServer()
        {
            bool success = false;
            Server.StartHosting(5000);
            Client.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Inconclusive("Connection failed, server rejected connection."); };
            Client.AttemptConnection("127.0.0.1", 5000, 1000);

            //Await connection to the server
            while (!success)
                Thread.Sleep(0);

            //Send a client message to the server
            

        }

        [TestMethod]
        [Timeout(1500)]
        public void TestSendingToClient()
        {
            bool connected = false;
            bool success = false;
            Server.StartHosting(5000);
            Client.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Inconclusive("Connection failed, server rejected connection."); };
            Client.OnConnectionSuccess += (IPAddress ipAddress) => { connected = true; };
            Client.NetworkMessageReceived += (PacketHeaders packetHeader, byte[] message) =>
            {
                if (packetHeader == PacketHeaders.NETWORKING_TEST)
                {
                    if (Encoding.ASCII.GetString(message) != "CORRECT")
                        Assert.Fail($"Recieved message did not contain correct content, content recieved: '{Encoding.ASCII.GetString(message)}'");
                    success = true;
                }
            };
            Client.AttemptConnection("127.0.0.1", 5000, 1000);

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
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestBanning()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(1500)]
        public void TestNetworkedEvent()
        {
            throw new NotImplementedException();
        }

    }
}
