using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Generators;
using CorgEng.UserInterface.Attributes;
using CorgEng.UserInterface.Events;
using CorgEng.UserInterface.Systems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Tests.UserInterfaceTests.UnitTests
{
    [TestClass]
    public class UserInterfaceButtonTests
    {

        [UsingDependency]
        private static IUserInterfaceXmlLoader UserInterfaceXmlLoader;

        private static bool TestPassed = false;

        [UserInterfaceCodeCallback("PassTest")]
        public static void OnButtonPressed(IUserInterfaceComponent userInterfaceComponent)
        {
            TestPassed = true;
        }

        private static bool setup = false;

        [TestInitialize]
        public void SetupTests()
        {
            if (!setup)
            {
                setup = true;
                UserInterfaceClickSystem clickSystem = new UserInterfaceClickSystem();
                clickSystem.SystemSetup();
            }
        }

        [TestMethod]
        [Timeout(5000)]
        public void TestUserInterfaceButton()
        {
            //Verify assumptions
            if (!File.Exists("UserInterfaceTests/UnitTests/Content/UserInterfaceButton.xml"))
                Assert.Inconclusive("Specified filepath does not exist.");
            if (UserInterfaceXmlLoader == null)
                Assert.Inconclusive("User interface XML loader dependency was not injected.");
            //Check for loading errors
            IUserInterfaceComponent Root = UserInterfaceXmlLoader.LoadUserInterface("UserInterfaceTests/UnitTests/Content/UserInterfaceButton.xml");
            IUserInterfaceComponent button = Root.GetChildren().First();
            new UserInterfaceClickEvent().Raise(button.ComponentHolder);
            while (!TestPassed)
                Thread.Yield();
        }

    }
}
