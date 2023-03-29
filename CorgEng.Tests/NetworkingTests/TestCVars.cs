using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Components.ComponentVariables.Networking;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Clients;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.Packets;
using CorgEng.Networking.Components;
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

    public class ExampleComponent : Component
    {

        public NetCVar<int, ExampleComponent> exampleNetvar = new NetCVar<int, ExampleComponent>(5);

    }


    [TestClass]
    internal class TestCVars
    {

        [UsingDependency]
        private static INetworkingServer Server;

        [UsingDependency]
        private static INetworkingClient Client;

        [UsingDependency]
        private static INetworkMessageFactory MessageFactory;

        [UsingDependency]
        private static IEntityFactory EntityFactory;

        [UsingDependency]
        private static ILogger Logger;

        [TestCleanup]
        public void AfterTest()
        {
            Server.Cleanup();
            Client.Cleanup();
            Logger?.WriteLine("TEST COMPLETED", LogType.DEBUG);
        }

        [TestInitialize]
        public void TestStart()
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
        }

        [TestMethod]
        public void TestNetVarModification()
        {
            // Create the entity
            EntityFactory.CreateEmptyEntity(entity => {
                entity.AddComponent(new NetworkTransformComponent());
                entity.AddComponent(new ExampleComponent());
            });
            //
        }

    }
}
