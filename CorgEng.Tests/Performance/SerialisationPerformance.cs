//#define PERFORMANCE_TEST

using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
using CorgEng.Tests.NetworkingTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Tests.Performance
{
    [TestClass]
    public class SerialisationPerformance
    {

        [UsingDependency]
        private static IEntityFactory EntityFactory;

        [UsingDependency]
        private static IPrototypeManager PrototypeManager;

        private const int TEST_TIME = 5000;

        [TestMethod]
        public void TestEntityToPrototypePerformance()
        {
#if !PERFORMANCE_TEST
            Assert.Inconclusive("Test not executed. Please enable PERFORMANCE_TEST define in order to test performance.");
#endif
            //Create an entity
            IEntity entity = EntityFactory.CreateEmptyEntity(null);
            TestComponent testComponent = new TestComponent();
            testComponent.Integer = 59;
            testComponent.Text = "Hello World!";
            testComponent.Double = 3.14159265;
            entity.AddComponent(testComponent);

            //Perform the tests
            bool running = true;
            int runs = 0;
            Thread thread = new Thread(() => {
                while (running)
                {
                    //Perform a run
                    PrototypeManager.GetPrototype(entity, false);
                    //Run completed
                    runs++;
                }
            });
            thread.Start();
            Thread.Sleep(TEST_TIME);
            running = false;
            //Process results
            Assert.Inconclusive($"Performed {runs} prototype gets in {TEST_TIME}ms at a rate of {runs / (TEST_TIME / 1000f)}/s");
        }

        [TestMethod]
        public void TestPrototypeToEntityPerformance()
        {
#if !PERFORMANCE_TEST
            Assert.Inconclusive("Test not executed. Please enable PERFORMANCE_TEST define in order to test performance.");
#endif
            //Create an entity
            IEntity entity = EntityFactory.CreateEmptyEntity(null);
            TestComponent testComponent = new TestComponent();
            testComponent.Integer = 59;
            testComponent.Text = "Hello World!";
            testComponent.Double = 3.14159265;
            entity.AddComponent(testComponent);
            //Serialize the entity's prototype
            IPrototype collectedPrototype = PrototypeManager.GetPrototype(entity, false);

            //Perform the tests
            bool running = true;
            int runs = 0;
            Thread thread = new Thread(() => {
                while (running)
                {
                    //Perform a run, don't care about the identifier
                    collectedPrototype.CreateEntityFromPrototype();
                    //Run completed
                    runs++;
                }
            });
            thread.Start();
            Thread.Sleep(TEST_TIME);
            running = false;
            //Process results
            Assert.Inconclusive($"Performed {runs} instantiations in {TEST_TIME}ms at a rate of {runs / (TEST_TIME / 1000f)}/s");
        }

        [TestMethod]
        public void TestDeserialisationPerformance()
        {
#if !PERFORMANCE_TEST
            Assert.Inconclusive("Test not executed. Please enable PERFORMANCE_TEST define in order to test performance.");
#endif

            //Create an entity
            IEntity entity = EntityFactory.CreateEmptyEntity(null);
            TestComponent testComponent = new TestComponent();
            testComponent.Integer = 59;
            testComponent.Text = "Hello World!";
            testComponent.Double = 3.14159265;
            entity.AddComponent(testComponent);
            //Serialize the entity's prototype
            IPrototype collectedPrototype = PrototypeManager.GetPrototype(entity, false);
            byte[] byteStream = collectedPrototype.SerializePrototype();

            //Perform the tests
            bool running = true;
            int runs = 0;
            Thread thread = new Thread(() => {
                while (running)
                {
                    //Perform a run
                    PrototypeManager.GetPrototype(byteStream);
                    //Run completed
                    runs++;
                }
            });
            thread.Start();
            Thread.Sleep(TEST_TIME);
            running = false;
            //Process results
            Assert.Inconclusive($"Performed {runs} deserialisations in {TEST_TIME}ms at a rate of {runs / (TEST_TIME / 1000f)}/s");
        }

        [TestMethod]
        public void TestSerialisationPerformance()
        {
#if !PERFORMANCE_TEST
            Assert.Inconclusive("Test not executed. Please enable PERFORMANCE_TEST define in order to test performance.");
#endif

            //Create an entity
            IEntity entity = EntityFactory.CreateEmptyEntity(null);
            TestComponent testComponent = new TestComponent();
            testComponent.Integer = 59;
            testComponent.Text = "Hello World!";
            testComponent.Double = 3.14159265;
            entity.AddComponent(testComponent);
            //Serialize the entity's prototype
            IPrototype collectedPrototype = PrototypeManager.GetPrototype(entity, false);

            //Perform the tests
            bool running = true;
            int runs = 0;
            Thread thread = new Thread(() => {
                while (running)
                {
                    //Perform a run
                    collectedPrototype.SerializePrototype();
                    //Run completed
                    runs++;
                }
            });
            thread.Start();
            Thread.Sleep(TEST_TIME);
            running = false;
            //Process results
            Assert.Inconclusive($"Performed {runs} serialisations in {TEST_TIME}ms at a rate of {runs / (TEST_TIME / 1000f)}/s");
        }

    }
}
