using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.UserInterface.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.UserInterfaceTests.UnitTests
{
    [TestClass]
    public class UserInterfaceXmlLoaderTests
    {

        [UsingDependency]
        private static IUserInterfaceXmlLoader UserInterfaceXmlLoader;

        [TestMethod]
        public void TestDependencyImplementation()
        {
            Assert.IsNotNull(UserInterfaceXmlLoader);
        }

        /// <summary>
        /// Test for basic errors while parsing.
        /// Doesn't validate sensible contents of the loader, just
        /// if the loader did something.
        /// </summary>
        [DataTestMethod]
        [DataRow("UserInterfaceTests/UnitTests/Content/UserInterfaceExample.xml")]
        public void TestXmlLoadingErrors(string filepath)
        {
            //Verify assumptions
            if (!File.Exists(filepath))
                Assert.Inconclusive("Specified filepath does not exist.");
            if (UserInterfaceXmlLoader == null)
                Assert.Inconclusive("User interface XML loader dependency was not injected.");
            //Check for loading errors
            UserInterfaceXmlLoader.LoadUserInterface(filepath);
        }

    }
}
