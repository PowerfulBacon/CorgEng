using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CorgEng.Tests.EntityComponentSystem
{

    internal class OtherEvent : Event
    { }

    internal class TestEvent : Event
    { }

    internal class TestComponent : Component
    { }

    internal class TestEntitySystem : EntitySystem
    {

        public override void SystemSetup()
        {
            RegisterLocalEvent<TestComponent, TestEvent>(HandleTestEvent);
        }

        private void HandleTestEvent(Entity entity, TestComponent component, TestEvent eventDetails)
        {
            SignalTests.handlesReceieved++;
        }

    }

    [TestClass]
    public class SignalTests
    {

        internal static int handlesReceieved = 0;

        [TestMethod]
        public void TestSignalHandling()
        {
            //Setup a test entity system first
            TestEntitySystem entitySystem = new TestEntitySystem();
            entitySystem.SystemSetup();
            //Create a test entity
            Entity testEntity = new Entity();
            //Add a test component
            TestComponent testComponent = new TestComponent();
            testEntity.AddComponent(testComponent);
            //Test
            Assert.AreEqual(0, handlesReceieved);
            //Send a test signal
            new TestEvent().Raise(testEntity);
            Assert.AreEqual(1, handlesReceieved);
            new TestEvent().Raise(testEntity);
            Assert.AreEqual(2, handlesReceieved);
            new OtherEvent().Raise(testEntity);
            Assert.AreEqual(2, handlesReceieved);
            new TestEvent().Raise(testEntity);
            Assert.AreEqual(3, handlesReceieved);
        }

    }
}
