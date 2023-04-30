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
    public class TestCVars
    {

        [UsingDependency]
        private static IWorldFactory WorldFactory;

        [UsingDependency]
        private static INetworkMessageFactory MessageFactory;

        [UsingDependency]
        private static ILogger Logger;

        private IWorld serverWorld;
        private IWorld clientWorld;

        [TestInitialize]
        public void TestStart()
        {
            serverWorld = WorldFactory.CreateWorld();
            clientWorld = WorldFactory.CreateWorld();

            bool connected = false;
            bool success = false;
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
        }

        [TestMethod]
        public void TestNetVarModification()
        {
            // Create the entity
            IEntity createdEntity = serverWorld.EntityManager.CreateEmptyEntity(entity => {
                entity.AddComponent(new NetworkTransformComponent());
                entity.AddComponent(new ExampleComponent());
            });
            //
            Assert.Inconclusive("I haven't made this");
        }

    }
}
