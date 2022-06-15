﻿using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.GenericInterfaces.Networking.Networking;
using CorgEng.GenericInterfaces.Networking.Networking.Client;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
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

    public class TestComponent : Component
    {

        [NetworkSerialized]
        public int Integer { get; set; }

        [NetworkSerialized]
        public string Text { get; set; }

        [NetworkSerialized]
        public double Double { get; set; }

        public override bool SetProperty(string name, IPropertyDef property)
        {
            return false;
        }
    }

    [TestClass]
    public class PrototypeTests
    {

        [UsingDependency]
        private static IPrototypeManager PrototypeManager;

        [UsingDependency]
        private static INetworkingServer Server;

        [UsingDependency]
        private static INetworkingClient Client;

        [UsingDependency]
        private static ILogger Logger;

        [TestCleanup]
        public void AfterTest()
        {
            Server.Cleanup();
            Client.Cleanup();
        }

        [TestMethod]
        [Timeout(10000)]
        public void TestPrototypes()
        {
            bool success = false;
            Server.StartHosting(5000);
            Client.OnConnectionSuccess += (IPAddress ipAddress) => { success = true; };
            Client.OnConnectionFailed += (IPAddress ipAddress, DisconnectReason disconnectReason, string reasonText) => { Assert.Fail("Connection failed, server rejected connection."); };
            Client.AttemptConnection("127.0.0.1", 5000, 1000);

            while (!success)
                Thread.Sleep(0);

            //Create an entity
            IEntity entity = new Entity();
            TestComponent testComponent = new TestComponent();
            testComponent.Integer = 59;
            testComponent.Text = "Hello World!";
            testComponent.Double = 3.14159265;
            entity.AddComponent(testComponent);
            //Serialize the entity's prototype
            IPrototype collectedPrototype = PrototypeManager.GetPrototype(entity);
            byte[] serialisedPrototype = collectedPrototype.SerializePrototype();

            //Log the serialised prototype
            string logMessage = string.Join("|", serialisedPrototype);
            Logger.WriteLine(logMessage, LogType.DEBUG);

            //Deserialize the entity's prototype
            IPrototype deserialisedPrototype = PrototypeManager.GetProtoype(serialisedPrototype);
            IEntity createdEntity = deserialisedPrototype.CreateEntityFromPrototype();
            //Verify the entity is correct
            Assert.AreEqual(entity.Components.Count, createdEntity.Components.Count);
            TestComponent deserializedComponent = (TestComponent)createdEntity.Components[0];
            Assert.AreEqual(59, deserializedComponent.Integer);
            Assert.AreEqual("Hello World!", deserializedComponent.Text);
            Assert.AreEqual(3.14159265, deserializedComponent.Double);
        }

    }
}
