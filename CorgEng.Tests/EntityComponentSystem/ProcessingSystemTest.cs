using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.EntityComponentSystem.Systems;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CorgEng.Tests.EntityComponentSystem.TestEntitySystems;

namespace CorgEng.Tests.EntityComponentSystem
{
    [TestClass]
    public class ProcessingSystemTest
    {

        [UsingDependency]
        private static IWorldFactory WorldFactory;

        private class TestComponent : Component
        {

            public int timesProcessed = 0;

        }

        private class TestSystem : ProcessingSystem
        {

            public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

            protected override int ProcessDelay => 100;

            public volatile int timesSignalled = 0;

            public override void SystemSetup(IWorld world)
            {
                RegisterLocalEvent<TestComponent, TestEvent>((e, t, ev) => {
                    timesSignalled++;
                });
            }

        }


        [TestMethod]
        public void TestProcessingSystems()
        {
            IWorld world = WorldFactory.CreateWorld();
            CorgEngMain.PrimaryWorld = world;
            //Create an entity to process
            IEntity testEntity = world.EntityManager.CreateEmptyEntity(null);
            TestComponent testComponent = new TestComponent();
            testEntity.AddComponent(testComponent);
            TestSystem testSystem = world.EntitySystemManager.GetSingleton<TestSystem>();
            //Start processing
            testSystem.RegisterProcess<TestComponent>(testEntity, (e, t, d) => {
                t.timesProcessed++;
            });
            //Send some signals to check
            new TestEvent().Raise(testEntity);
            new TestEvent().Raise(testEntity);
            new TestEvent().Raise(testEntity);
            //Wait 1 second
            Thread.Sleep(5000);
            //Kill the test system off
            testSystem.Kill();
            //Assertions
            Assert.AreEqual(3, testSystem.timesSignalled);
            // Close enough
            Assert.IsTrue(testComponent.timesProcessed > 45 && testComponent.timesProcessed <= 51, $"TestComponent was processed {testComponent.timesProcessed} times.");
            Console.WriteLine($"TestComponent was processed {testComponent.timesProcessed} times.");
        }

    }
}
