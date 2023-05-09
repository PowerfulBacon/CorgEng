using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Tests.EntityComponentSystem
{
    [TestClass]
    public class TestEntitySystems : TestBase
    {

        [UsingDependency]
        private static IWorldFactory WorldFactory = null!;

        public class TestSystem : EntitySystem
        {

            public bool testCompleted = false;

            public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM | EntitySystemFlags.CLIENT_SYSTEM;

            public override void SystemSetup(IWorld world)
            {
                RegisterLocalEvent<TestComponent, TestEvent>((e, c, s) => {
                    testCompleted = true;
                });
            }
        }

        public class TestComponent : Component
        {

        }

        public class TestEvent : IEvent
        {

        }

        [TestMethod]
        public void TestEntitySystem()
        {
            IWorld world = WorldFactory.CreateWorld();
            IEntity entity = world.EntityManager.CreateEmptyEntity(e => {
                e.AddComponent(new TestComponent());
            });
            TestSystem system = world.EntitySystemManager.GetSingleton<TestSystem>();
            // Verify
            Assert.IsFalse(system.testCompleted);
            // Send the signal
            new TestEvent().Raise(entity);
            // Assertion
            Assert.IsTrue(system.testCompleted);
        }

        [TestMethod]
        [Timeout(200)]
        public void TestEntitySystemThreadProcessing()
        {
            IWorld world = WorldFactory.CreateWorld();
            IEntity entity = world.EntityManager.CreateEmptyEntity(e => {
                e.AddComponent(new TestComponent());
            });
            TestSystem system = world.EntitySystemManager.GetSingleton<TestSystem>();
            // Verify
            Assert.IsFalse(system.testCompleted);
            // Send the signal
            system.AcquireHighPriorityLock();
            new TestEvent().Raise(entity);
            system.ReleaseLock();
            while (!system.testCompleted)
                Thread.Sleep(10);
        }

        [TestMethod]
        public void TestSystemIsolation()
        {
            IWorld world = WorldFactory.CreateWorld();
            IWorld backgroundWorld = WorldFactory.CreateWorld();
            IEntity entity = world.EntityManager.CreateEmptyEntity(e => {
                e.AddComponent(new TestComponent());
            });
            TestSystem backgroundSystem = backgroundWorld.EntitySystemManager.GetSingleton<TestSystem>();
            Assert.IsFalse(backgroundSystem.testCompleted);
            new TestEvent().Raise(entity);
            Assert.IsFalse(backgroundSystem.testCompleted);
        }

    }
}
