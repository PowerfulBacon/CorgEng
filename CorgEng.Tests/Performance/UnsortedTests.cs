//#define PERFORMANCE_TEST

using CorgEng.EntityComponentSystem.Components;
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
    [DoNotParallelize]
    public class UnsortedTests
    {

        private class Test
        {
            
        }

        private class TestA : Test
        { }
        private class TestB : Test
        { }
        private class TestC : Test
        { }
        private class TestD : Test
        { }

        private const int TEST_TIME = 5000;

        private Dictionary<Type, Test> testDictionaries = new Dictionary<Type, Test>() {
            { typeof(TestA), new TestA() },
            { typeof(TestB), new TestB() },
            { typeof(TestC), new TestC() },
            { typeof(TestD), new TestD() },
        };

        private sealed class TestLookup<T>
            where T : Test
        {
            internal static T Tester = Activator.CreateInstance<T>();
        }

        [TestMethod]
        public void DictionaryLookup()
        {
#if !PERFORMANCE_TEST
            Assert.Inconclusive("Test not executed. Please enable PERFORMANCE_TEST define in order to test performance.");
#endif

            //Perform the tests
            bool running = true;
            int runs = 0;
            Thread thread = new Thread(() => {
                while (running)
                {
                    switch (runs % 4)
                    {
                        case 0:
                            Test thing = testDictionaries[typeof(TestA)];
                            break;
                        case 1:
                            Test thing1 = testDictionaries[typeof(TestB)];
                            break;
                        case 2:
                            Test thing2 = testDictionaries[typeof(TestC)];
                            break;
                        case 3:
                            Test thing3 = testDictionaries[typeof(TestD)];
                            break;
                    }
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
        public void TypeLookup()
        {
#if !PERFORMANCE_TEST
            Assert.Inconclusive("Test not executed. Please enable PERFORMANCE_TEST define in order to test performance.");
#endif

            //Perform the tests
            bool running = true;
            int runs = 0;
            Thread thread = new Thread(() => {
                while (running)
                {
                    switch (runs % 4)
                    {
                        case 0:
                            Test thing = TestLookup<TestA>.Tester;
                            break;
                        case 1:
                            Test thing1 = TestLookup<TestB>.Tester;
                            break;
                        case 2:
                            Test thing2 = TestLookup<TestC>.Tester;
                            break;
                        case 3:
                            Test thing3 = TestLookup<TestD>.Tester;
                            break;
                    }
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

    }
}
