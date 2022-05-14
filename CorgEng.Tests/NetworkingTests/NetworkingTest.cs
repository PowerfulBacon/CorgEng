using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Networking.Networking;
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

    }
}
