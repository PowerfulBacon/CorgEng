using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.DependencyInjection.Injection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CorgEng.Tests.DependencyInjection
{

    [TestClass]
    public class DependencyTests
    {

        private interface IDependency
        {
            bool CompleteTest();
        }

        [Dependency]
        private class BadDependency : IDependency
        {
            public bool CompleteTest() => false;
        }

        [Dependency]
        private class Dependency : IDependency
        {
            public bool CompleteTest() => true;
        }

        [UsingDependency]
        private static IDependency dependency;

        [TestMethod]
        public void TestDependency()
        {
            //Load dependency injection
            DependencyInjector.LoadDependencyInjection();
            //Execute test
            Assert.IsTrue(dependency.CompleteTest());
        }

    }
}
