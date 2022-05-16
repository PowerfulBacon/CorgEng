using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Injection;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace CorgEng.Tests.EntityComponentSystem
{

    internal class OtherEvent : Event
    {
        public override bool NetworkedEvent => false;
    }

    internal class TestEvent : Event
    {
        public int TestID { get; }

        public override bool NetworkedEvent => false;

        public TestEvent()
        {
            TestID = SignalTests.currentTestId;
        }
    }

    internal class TestComponent : Component
    {
        public override bool SetProperty(string name, IPropertyDef property)
        {
            return false;
        }
    }

    internal class SecondaryTestComponent : Component
    {
        public override bool SetProperty(string name, IPropertyDef property)
        {
            return false;
        }
    }

    internal class TestEntitySystem : EntitySystem
    {

        public override void SystemSetup()
        {
            RegisterLocalEvent<TestComponent, TestEvent>(HandleTestEvent);
            RegisterLocalEvent<SecondaryTestComponent, TestEvent>(HandleSecondaryTestEvent);
            RegisterGlobalEvent<TestEvent>(HandleGlobalEvent);
        }

        private void HandleTestEvent(Entity entity, TestComponent component, TestEvent eventDetails)
        {
            if (eventDetails.TestID != SignalTests.currentTestId)
            {
                Console.WriteLine($"Ignoring event due to invalid testID: {eventDetails.TestID} != {SignalTests.currentTestId}");
                return;
            }
            Assert.AreEqual(typeof(TestComponent), component.GetType(), "Handle secondary test event wait raised on an invalid component!");
            Console.WriteLine($"[LOCAL EVENT]: Current thread: {Thread.CurrentThread.Name} - {Environment.StackTrace}");
            SignalTests.handlesReceieved++;
            Console.WriteLine(SignalTests.handlesReceieved);
        }

        private void HandleSecondaryTestEvent(Entity entity, SecondaryTestComponent component, TestEvent eventDetails)
        {
            if (eventDetails.TestID != SignalTests.currentTestId)
            {
                Console.WriteLine($"Ignoring event due to invalid testID: {eventDetails.TestID} != {SignalTests.currentTestId}");
                return;
            }
            Assert.AreEqual(typeof(SecondaryTestComponent), component.GetType(), "Handle secondary test event wait raised on an invalid component!");
            Console.WriteLine($"[LOCAL EVENT]: Current thread: {Thread.CurrentThread.Name} - {Environment.StackTrace}");
            SignalTests.secondaryHandlesReceieved++;
            Console.WriteLine(SignalTests.secondaryHandlesReceieved);
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

        /// <summary>
        /// DONT USE DEPENDNCY INJECTION TO FORCE THIS
        /// </summary>
        private static ConsoleLogger Logger = new ConsoleLogger();

        internal volatile static int handlesReceieved = 0;

        internal volatile static int secondaryHandlesReceieved = 0;

        internal volatile static bool passedGlobalTest = false;

        private volatile static TestEntitySystem entitySystem;

        internal volatile static int currentTestId = 0;

        public SignalTests()
        {
            if (entitySystem == null)
            {
                Logger?.WriteLine("Setting up test");
                entitySystem = new TestEntitySystem();
                entitySystem.SystemSetup();
            }
        }

        [TestCleanup]
        public void CleanupTests()
        {
            handlesReceieved = 0;
            secondaryHandlesReceieved = 0;
            passedGlobalTest = false;
            currentTestId++;
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestComponentRemoval()
        {
            Assert.AreEqual(0, handlesReceieved, "INCORRECT TEST CONFIGURATION");
            Logger?.WriteLine($"Current thread: {Thread.CurrentThread.ManagedThreadId}. TestID: {currentTestId}");
            //Create a test entity
            Entity testEntity = new Entity();
            //Add a test component
            TestComponent testComponent = new TestComponent();
            testEntity.AddComponent(testComponent);
            //Remove the component
            testEntity.RemoveComponent(testComponent, false);
            //Test sending signal no longer works
            //Send a test signal
            new TestEvent().Raise(testEntity);
            Thread.Sleep(50);
            Assert.AreEqual(0, handlesReceieved, "Should not have receieved a signal from removed component.");
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestComponentRemovalIsolation()
        {
            Assert.AreEqual(0, handlesReceieved, "INCORRECT TEST CONFIGURATION");
            Assert.AreEqual(0, secondaryHandlesReceieved, "INCORRECT TEST CONFIGURATION");
            Logger?.WriteLine($"Current thread: {Thread.CurrentThread.ManagedThreadId}. TestID: {currentTestId}");
            //Create a test entity
            Entity testEntity = new Entity();
            //Add a test component
            TestComponent testComponent = new TestComponent();
            testEntity.AddComponent(testComponent);
            SecondaryTestComponent secondaryTestComponent = new SecondaryTestComponent();
            testEntity.AddComponent(secondaryTestComponent);
            //Remove the component
            testEntity.RemoveComponent(testComponent, false);
            //Test sending signal no longer works
            //Send a test signal
            new TestEvent().Raise(testEntity);
            Thread.Sleep(50);
            Assert.AreEqual(0, handlesReceieved, "Should not have receieved a signal from removed component.");
            while (secondaryHandlesReceieved != 1)
                Thread.Sleep(1);
            Assert.AreEqual(1, secondaryHandlesReceieved, "Should have receieved a signal from remaining component.");
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestSignalHandling()
        {
            Assert.AreEqual(0, handlesReceieved, "INCORRECT TEST CONFIGURATION");
            Logger?.WriteLine($"Current thread: {Thread.CurrentThread.ManagedThreadId}. TestID: {currentTestId}");
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
            Logger?.WriteLine("Passed 1");
            new TestEvent().Raise(testEntity);
            while (handlesReceieved != 2)
                Thread.Sleep(1);
            Logger?.WriteLine("Passed 2");
            new OtherEvent().Raise(testEntity);
            Thread.Sleep(50);  //Sleep to account for multi-threading
            Assert.AreEqual(2, handlesReceieved);
            new TestEvent().Raise(testEntity);
            while (handlesReceieved != 3)
                Thread.Sleep(1);
            Logger?.WriteLine("Passed 3");
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestGlobalSignalHandling()
        {
            Assert.IsFalse(passedGlobalTest, "INCORRECT TEST CONFIGURATION");
            Logger?.WriteLine($"Current thread: {Thread.CurrentThread.ManagedThreadId}. TestID: {currentTestId}");
            new OtherEvent().RaiseGlobally();
            Thread.Sleep(50);
            Assert.IsFalse(passedGlobalTest);
            new TestEvent().RaiseGlobally();
            while (!passedGlobalTest)
                Thread.Sleep(1);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestMultipleSignalHandling()
        {
            Assert.AreEqual(0, handlesReceieved, "INCORRECT TEST CONFIGURATION");
            Logger?.WriteLine($"Current thread: {Thread.CurrentThread.ManagedThreadId}. TestID: {currentTestId}");
            //Create a test entity
            Entity testEntity = new Entity();
            //Add a test component
            TestComponent testComponent = new TestComponent();
            testEntity.AddComponent(testComponent);
            SecondaryTestComponent secondaryTestComponent = new SecondaryTestComponent();
            testEntity.AddComponent(secondaryTestComponent);
            //Test
            Assert.AreEqual(0, handlesReceieved);
            //Send a test signal
            new TestEvent().Raise(testEntity);
            while (handlesReceieved != 1 && secondaryHandlesReceieved != 1)
                Thread.Sleep(1);
        }

    }
}
