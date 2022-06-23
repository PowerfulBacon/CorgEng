using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
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
    public class EntityCommunicatorTests
    {

        [UsingDependency]
        private static IEntityCommunicator EntityCommunicator;

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
            Logger?.WriteLine("TEST COMPLETED", LogType.DEBUG);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestEntityCommunication()
        {
            //Alright, connection established. Lets communicate an entity
            IEntity testEntity = new Entity();
            TestComponent testComponent = new TestComponent();
            testComponent.Text = "test";
            testComponent.DontSerialise = 5.31262;
            testComponent.Double = 1.252345;
            testComponent.Integer = 52;
            testEntity.AddComponent(testComponent);
            //Communicate the entity
            byte[] serialisedEntity = EntityCommunicator.SerializeEntity(testEntity);
            //Deserialise the entity
            IEntity deserialisedEntity = EntityCommunicator.DeserialiseEntity(serialisedEntity).Result;
            //Perform tests
            Assert.AreEqual(testEntity.Identifier, deserialisedEntity.Identifier);
            bool hasTestComponent = false;

            foreach (IComponent component in deserialisedEntity.Components)
            {
                if (component is TestComponent addedTestComponent)
                {
                    //Add a test component
                    hasTestComponent = true;
                    //Check component values
                    Assert.AreEqual("test", addedTestComponent.Text);
                    Assert.AreEqual(5.31262, addedTestComponent.DontSerialise);
                    Assert.AreEqual(1.252345, addedTestComponent.Double);
                    Assert.AreEqual(52, addedTestComponent.Integer);
                }
                else
                {
                    Assert.Fail("TestComponent has an invalid component.");
                }
            }
            //Verify components
            Assert.IsTrue(hasTestComponent);
        }

    }
}
