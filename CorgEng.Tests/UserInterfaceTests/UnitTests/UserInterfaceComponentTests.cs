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
        [DataTestMethod]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.LEFT, 0, 500, 500)]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.LEFT, 50, 250, 200)]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.RIGHT, 50, 50, 0)]
        [DataRow(AnchorDirections.RIGHT, AnchorDirections.RIGHT, 300, 50, 250)]
        public void TestUserInterfacePixelScaling(
            AnchorDirections leftAnchorDir,
            AnchorDirections rightAnchorDir,
            double leftAnchorAmount,
            double rightAnchorAmount,
            double expectedWidth)
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
            IAnchorDetails leftAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(leftAnchorDir, AnchorUnits.PIXELS, leftAnchorAmount);
            IAnchorDetails rightAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(rightAnchorDir, AnchorUnits.PIXELS, rightAnchorAmount);
            //Craete the anchor
            IAnchor anchor = AnchorFactory.CreateAnchor(leftAnchorDetails, rightAnchorDetails, topAnchorDetails, bottomAnchorDetails);
            //Create a generic user interface component
            IUserInterfaceComponent userInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(anchor);
            //Ensure user interface minimum scale correctness
            Assert.AreEqual(0, userInterfaceComponent.MinimumPixelHeight);
            Assert.AreEqual(expectedWidth, userInterfaceComponent.MinimumPixelWidth);
        }

        /// <summary>
        /// This test verifies that the parent's have the correct minimum width and height
        /// based off of their children component's width and height.
        /// </summary>
        /// <assumptions>
        /// Assumes that dependencies have been created and are loaded correctly.
        /// Assumes that components can correctly calculate their own minimum width and height.
        /// </assumptions>
        [DataTestMethod]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.LEFT, 0, 50, 50)]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.LEFT, 50, 250, 250)]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.LEFT, 100, 100, 100)]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.RIGHT, 100, 100, 200)] //Requires 100 left and 100 right
        [DataRow(AnchorDirections.LEFT, AnchorDirections.RIGHT, 0, 0, 0)]
        public void TestUserInterfaceParentPixelScaling(
            AnchorDirections leftChildAnchorDir,
            AnchorDirections rightChildAnchorDir,
            double leftChildAnchorAmount,
            double rightChildAnchorAmount,
            double expectedParentWidth)
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
            IAnchorDetails childLeftAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(leftChildAnchorDir, AnchorUnits.PIXELS, leftChildAnchorAmount);
            IAnchorDetails childRightAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(rightChildAnchorDir, AnchorUnits.PIXELS, rightChildAnchorAmount);
            //Craete the anchor
            IAnchor childAnchor = AnchorFactory.CreateAnchor(childLeftAnchorDetails, childRightAnchorDetails, childTopAnchorDetails, childBottomAnchorDetails);
            //Create a generic user interface component
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(parentAnchor);
            //Create another generic user interface component
            IUserInterfaceComponent childUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(parentUserInterfaceComponent, childAnchor);
            //Check parent minimum width
            Assert.AreEqual(200, parentUserInterfaceComponent.MinimumPixelHeight);  //Should be 200 as child component requires 100 space above, and 100 space below
            Assert.AreEqual(expectedParentWidth, parentUserInterfaceComponent.MinimumPixelWidth);   //Should be 250 as child component requires 250 space left and none right.
        }

    }
}
