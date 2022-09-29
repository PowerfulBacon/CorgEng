//#define PERFORMANCE_TEST

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

namespace CorgEng.Tests.Performance
{
    /// <summary>
    /// NOTE:
    /// THESE TESTS DONT WORK WHEN RAN TOGETHER.
    /// These are just for checking speed, not actual tests so fixing that
    /// isn't a priority.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class EventProcessing
    {

        private static int EventsHandled = 0;
        private static int LateHandles = 0;
        private static bool Running = false;

        private class TestEvent : IEvent
        { }

        private class TestEntitySystem : EntitySystem
        {
            public override EntitySystemFlags SystemFlags => EntitySystemFlags.HOST_SYSTEM;

            public override void SystemSetup()
            {
                RegisterGlobalEvent<TestEvent>(HandleEvent);
            }

            private void HandleEvent(TestEvent testEvent)
            {
                EventsHandled++;
                if (!Running)
                {
                    LateHandles++;
                }
            }

        }

        private const int TEST_TIME = 5000;

        [TestMethod]
        public void TestAsyncEventPerformance()
        {
#if !PERFORMANCE_TEST
            Assert.Inconclusive("Test not executed. Please enable PERFORMANCE_TEST define in order to test performance.");
#endif
            EventsHandled = 0;
            LateHandles = 0;
            TestEntitySystem testEntitySystem = new TestEntitySystem();
            testEntitySystem.SystemSetup();

            //Perform the tests
            Running = true;
            int runs = 0;
            Thread thread = new Thread(() => {
                while (Running)
                {
                    //Perform a run
                    new TestEvent().RaiseGlobally();
                    //Run completed
                    runs++;
                }
            });
            thread.Start();
            Thread.Sleep(TEST_TIME);
            Running = false;
            //Kill the system to flush the queue
            testEntitySystem.Kill();
            //Process results
            Assert.Inconclusive($"Fired {runs} events, handled {EventsHandled} events ({LateHandles} were handled late.). Handling rate: {(double)TEST_TIME/EventsHandled}ms per event (Avg) ; {EventsHandled / (TEST_TIME / 1000f)}/s");
        }

        [TestMethod]
        public void TestSynchronousEventPerformance()
        {
#if !PERFORMANCE_TEST
            Assert.Inconclusive("Test not executed. Please enable PERFORMANCE_TEST define in order to test performance.");
#endif
            EventsHandled = 0;
            LateHandles = 0;
            TestEntitySystem testEntitySystem = new TestEntitySystem();
            testEntitySystem.SystemSetup();

            //Perform the tests
            Running = true;
            int runs = 0;
            Thread thread = new Thread(() => {
                while (Running)
                {
                    //Perform a run
                    new TestEvent().RaiseGlobally(true);
                    //Run completed
                    runs++;
                }
            });
            thread.Start();
            Thread.Sleep(TEST_TIME);
            Running = false;
            //Kill the system to flush the queue
            testEntitySystem.Kill();
            //Process results
            Assert.Inconclusive($"Fired {runs} events, handled {EventsHandled} events. Handling rate: {(double)TEST_TIME / EventsHandled}ms per event (Avg) ; {EventsHandled / (TEST_TIME / 1000f)}/s");
        }

    }
}
