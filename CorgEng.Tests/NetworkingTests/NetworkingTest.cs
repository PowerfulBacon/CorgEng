using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
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
            Client.OnConnectionSuccess += (IPAddress ipAddress) => { success = true; };
            Client.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Fail("Connection failed, server rejected connection."); };
            Client.AttemptConnection("127.0.0.1", 5000, 1000);

            //Await connection to the serve
            while (!success)
                Thread.Sleep(0);

            //Send a client message to the server
            throw new NotImplementedException();

        }

        [TestMethod]
        public void TestSendingToClient()
        {
            throw new NotImplementedException();
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
        public void TestNetworkedEvent()
        {
            throw new NotImplementedException();
        }

    }
}
