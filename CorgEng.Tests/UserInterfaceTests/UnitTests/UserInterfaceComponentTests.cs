using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Injection;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.UserInterfaceTests.UnitTests
{
    [TestClass]
    public class UserInterfaceComponentTests
    {

        [UsingDependency]
        private static IUserInterfaceComponentFactory UserInterfaceComponentFactory;

        [UsingDependency]
        private static IAnchorFactory AnchorFactory;

        [UsingDependency]
        private static IAnchorDetailFactory AnchorDetailFactory;

        public UserInterfaceComponentTests()
        {
            //Load dependency injection, if required.
            if (!DependencyInjector.InjectionCompleted)
                DependencyInjector.LoadDependencyInjection();
        }

        /// <summary>
        /// This test verifies that the dependencies have been implemented.
        /// </summary>
        /// <assumptions>
        /// Assumes that dependency injection is working and functioning.
        /// </assumptions>
        [TestMethod]
        public void TestDependenciesCreated()
        {
            Assert.IsNotNull(UserInterfaceComponentFactory, "User interface component factory hasn't been implemented");
            Assert.IsNotNull(AnchorFactory, "Anchor factory hasn't been implemented");
            Assert.IsNotNull(AnchorDetailFactory, "Anchor detail factory hasn't been implemented");
        }

        /// <summary>
        /// This test verifies that components can calculate their own minimum width and height
        /// when they have no children.
        /// </summary>
        /// <assumptions>
        /// Assumes that dependencies have been created and are loaded correctly.
        /// </assumptions>
        [TestMethod]
        public void TestUserInterfacePixelScaling()
        {
            //Verify assumptions
            if (AnchorDetailFactory == null)
                Assert.Inconclusive("Anchor detail factory not located.");
            if (AnchorFactory == null)
                Assert.Inconclusive("Anchor factory not located.");
            if (UserInterfaceComponentFactory == null)
                Assert.Inconclusive("User interface factory not located.");
            //Create the anchor details
            IAnchorDetails topAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 100);
            IAnchorDetails bottomAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 100);
            IAnchorDetails leftAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 50);
            IAnchorDetails rightAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 250);
            //Craete the anchor
            IAnchor anchor = AnchorFactory.CreateAnchor(leftAnchorDetails, rightAnchorDetails, topAnchorDetails, bottomAnchorDetails);
            //Create a generic user interface component
            IUserInterfaceComponent userInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(anchor);
            //Ensure user interface minimum scale correctness
            Assert.AreEqual(0, userInterfaceComponent.MinimumPixelHeight);
            Assert.AreEqual(200, userInterfaceComponent.MinimumPixelWidth);
        }

        /// <summary>
        /// This test verifies that the parent's have the correct minimum width and height
        /// based off of their children component's width and height.
        /// </summary>
        /// <assumptions>
        /// Assumes that dependencies have been created and are loaded correctly.
        /// Assumes that components can correctly calculate their own minimum width and height.
        /// </assumptions>
        [TestMethod]
        public void TestUserInterfaceParentPixelScaling()
        {
            //Verify assumptions
            if (AnchorDetailFactory == null)
                Assert.Inconclusive("Anchor detail factory not located.");
            if (AnchorFactory == null)
                Assert.Inconclusive("Anchor factory not located.");
            if (UserInterfaceComponentFactory == null)
                Assert.Inconclusive("User interface factory not located.");
            //Create the parent anchor details
            IAnchorDetails parentTopAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0);
            IAnchorDetails parentBottomAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0);
            IAnchorDetails parentLeftAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0);
            IAnchorDetails parentRightAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0);
            //Craete the parent anchor
            IAnchor parentAnchor = AnchorFactory.CreateAnchor(parentLeftAnchorDetails, parentRightAnchorDetails, parentTopAnchorDetails, parentBottomAnchorDetails);
            //Create the anchor details
            IAnchorDetails childTopAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 100);
            IAnchorDetails childBottomAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 100);
            IAnchorDetails childLeftAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 50);
            IAnchorDetails childRightAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 250);
            //Craete the anchor
            IAnchor childAnchor = AnchorFactory.CreateAnchor(childLeftAnchorDetails, childRightAnchorDetails, childTopAnchorDetails, childBottomAnchorDetails);
            //Create a generic user interface component
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(parentAnchor);
            //Create another generic user interface component
            IUserInterfaceComponent childUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(parentUserInterfaceComponent, childAnchor);
            //Verify assumptions
            if (childUserInterfaceComponent.MinimumPixelHeight != 0)
                Assert.Inconclusive("Assumption verification failed: Minimum pixel height of child component is not 0.");
            if (childUserInterfaceComponent.MinimumPixelWidth != 200)
                Assert.Inconclusive("Assumption verification failed: Minimum pixel width of child component is not 200.");
            //Check parent minimum width
            Assert.AreEqual(200, parentUserInterfaceComponent.MinimumPixelHeight);  //Should be 200 as child component requires 100 space above, and 100 space below
            Assert.AreEqual(250, parentUserInterfaceComponent.MinimumPixelWidth);   //Should be 250 as child component requires 250 space left and none right.
        }

    }
}
