using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

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
            RegisterGlobalEvent<TestEvent>(HandleGlobalEvent);
        }

        private void HandleTestEvent(Entity entity, TestComponent component, TestEvent eventDetails)
        {
            Assert.AreEqual(typeof(TestComponent), component.GetType());
            Console.WriteLine($"[LOCAL EVENT]: Current thread: {Thread.CurrentThread.Name} - {Environment.StackTrace}");
            SignalTests.handlesReceieved++;
            Console.WriteLine(SignalTests.handlesReceieved);
        }

        private void HandleGlobalEvent(TestEvent globalEvent)
        {
            Console.WriteLine($"[GLOBAL EVENT]: Current thread: {Thread.CurrentThread.Name}");
            SignalTests.passedGlobalTest = true;
        }

    }

    [TestClass]
    public class SignalTests
    {

        internal volatile static int handlesReceieved = 0;

        internal volatile static bool passedGlobalTest = false;

        private volatile static TestEntitySystem entitySystem;
        
        public SignalTests()
        {
            if (entitySystem == null)
            {
                Console.WriteLine("Setting up test");
                entitySystem = new TestEntitySystem();
                entitySystem.SystemSetup();
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestSignalHandling()
        {
            Console.WriteLine($"Current thread: {Thread.CurrentThread.ManagedThreadId}");
            //Create a test entity
            Entity testEntity = new Entity();
            //Add a test component
            TestComponent testComponent = new TestComponent();
            testEntity.AddComponent(testComponent);
            //Test
            Assert.AreEqual(0, handlesReceieved);
            //Send a test signal
            new TestEvent().Raise(testEntity);
            while (handlesReceieved != 1)
                Thread.Sleep(1);
            Console.WriteLine("Passed 1");
            new TestEvent().Raise(testEntity);
            while (handlesReceieved != 2)
                Thread.Sleep(1);
            Console.WriteLine("Passed 2");
            new OtherEvent().Raise(testEntity);
            Thread.Sleep(50);  //Sleep to account for multi-threading
            Assert.AreEqual(2, handlesReceieved);
            new TestEvent().Raise(testEntity);
            while (handlesReceieved != 3)
                Thread.Sleep(1);
            Console.WriteLine("Passed 3");
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestGlobalSignalHandling()
        {
            Console.WriteLine($"Current thread: {Thread.CurrentThread.ManagedThreadId}");
            passedGlobalTest = false;
            new OtherEvent().RaiseGlobally();
            Thread.Sleep(50);
            Assert.IsFalse(passedGlobalTest);
            new TestEvent().RaiseGlobally();
            while (!passedGlobalTest)
                Thread.Sleep(1);
        }

    }
}
