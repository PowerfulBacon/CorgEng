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

namespace CorgEng.Tests.EntityComponentSystem
{
    [TestClass]
    public class ProcessingSystemTest
    {

        [UsingDependency]
        private static IEntityFactory EntityFactory;

        private class TestComponent : Component
        {

            public int timesProcessed = 0;

        }

        private class TestSystem : ProcessingSystem
        {

            public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

            protected override int ProcessDelay => 99;

            public volatile int timesSignalled = 0;

            public override void SystemSetup()
            {
                RegisterLocalEvent<TestComponent, TestEvent>((e, t, ev) => {
                    timesSignalled++;
                });
            }

        }


        [TestMethod]
        public void TestProcessingSystems()
        {
            //Start the test system
            TestSystem testSystem = new TestSystem();
            testSystem.SystemSetup();
            //Create an entity to process
            IEntity testEntity = EntityFactory.CreateEmptyEntity(null);
            TestComponent testComponent = new TestComponent();
            testEntity.AddComponent(testComponent);
            //Start processing
            testSystem.RegisterProcess<TestComponent>(testEntity, (e, t, d) => {
                t.timesProcessed++;
            });
            //Send some signals to check
            new TestEvent().Raise(testEntity);
            new TestEvent().Raise(testEntity);
            new TestEvent().Raise(testEntity);
            //Wait 1 second
            Thread.Sleep(1000);
            //Kill the test system off
            testSystem.Kill();
            //Assertions
            Assert.AreEqual(3, testSystem.timesSignalled);
            Assert.IsTrue(testComponent.timesProcessed > 0);
        }

    }
}
