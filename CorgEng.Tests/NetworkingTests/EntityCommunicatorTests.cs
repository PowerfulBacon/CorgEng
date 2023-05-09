using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Implementations.Deletion;
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
    public class EntityCommunicatorTests : TestBase
    {

        [UsingDependency]
        private static IEntityCommunicatorFactory EntityCommunicatorFactory;

        [UsingDependency]
        private static ILogger Logger;

        [UsingDependency]
        private static IWorldFactory WorldFactory;

        [TestMethod]
        [Timeout(1000)]
        public void TestEntityCommunication()
        {
            IWorld world = WorldFactory.CreateWorld();
            IEntityCommunicator EntityCommunicator = EntityCommunicatorFactory.CreateEntityCommunicator(world);
            //Alright, connection established. Lets communicate an entity
            IEntity testEntity = world.EntityManager.CreateEmptyEntity(null);
            TestComponent testComponent = new TestComponent();
            testComponent.Text = "test";
            testComponent.DontSerialise = 5.31262;
            testComponent.Double = 1.252345;
            testComponent.Integer = 52;
            testEntity.AddComponent(testComponent);
            //Communicate the entity
            byte[] serialisedEntity = EntityCommunicator.SerializeEntity(testEntity);
            //Clear entity manager, so it doesn't know about it
            world.EntityManager.RemoveEntity(testEntity);
            //Deserialise the entity
            IEntity deserialisedEntity = EntityCommunicator.DeserialiseEntity(serialisedEntity).Result;
            //Perform tests
            Assert.AreEqual(testEntity.Identifier, deserialisedEntity.Identifier);
            bool hasTestComponent = false;

            foreach (IComponent component in deserialisedEntity.Components)
            {
                if (component is DeleteableComponent)
                    continue;
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
