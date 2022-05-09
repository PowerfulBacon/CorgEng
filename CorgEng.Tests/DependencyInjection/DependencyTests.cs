using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.DependencyInjection.Injection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CorgEng.Tests.DependencyInjection
{

    /// <summary>
    /// Dependency injection overiding is required for these tests to run, so no point in testing it.
    /// </summary>
    [TestClass]
    public class DependencyTests
    {

        private interface IDependency
        {
            bool CompleteTest();
        }

        [Dependency(priority = 4)]
        private class BadDependency : IDependency
        {
            public bool CompleteTest() => false;
        }

        [Dependency(priority = 5)]
        private class Dependency : IDependency
        {
            public bool CompleteTest() => true;
        }

        [UsingDependency]
        private static IDependency dependency;

        [TestMethod]
        public void TestDependency()
        {
            //Execute test
            Assert.IsTrue(dependency.CompleteTest());
        }

    }
}
