//#define PERFORMANCE_TEST

using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.PrototypeManager;
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
    public class ThreadTest
    {

        [UsingDependency]
        private static ILogger Logger = null!;

        private const int TEST_TIME = 100;

        [TestMethod]
        public void TestEntityToPrototypePerformance()
        {
#if !PERFORMANCE_TEST
            Assert.Inconclusive("Test not executed. Please enable PERFORMANCE_TEST define in order to test performance.");
#endif
            for (int i = 1; i < 64; i++)
            {
                int completed = TestWithThreadCount(i);
                Logger.WriteLine($"Thread count: {i}. Completed: {completed}.", LogType.TEMP);
            }
        }


        public int TestWithThreadCount(int number)
        {
            double total = 0;
            int completed = 0;
            bool running = true;
            for (int i = 0; i < number; i++)
            {
                Thread t = new Thread(() =>
                {
                    while (running)
                    {
                        Random random = new Random();
                        double calculated = Math.Sqrt(random.NextDouble());
                        total += calculated;
                        Interlocked.Increment(ref completed);
                    }
                });
                t.Start();
            }
            Thread.Sleep(TEST_TIME);
            running = false;
            return completed;
        }

    }
}
