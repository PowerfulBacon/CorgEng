using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Injection;
using CorgEng.GenericInterfaces.EntityComponentSystem;
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
    public class UserInterfaceComponentTests : TestBase
    {

        [UsingDependency]
        private static IUserInterfaceComponentFactory UserInterfaceComponentFactory;

        [UsingDependency]
        private static IAnchorFactory AnchorFactory;

        [UsingDependency]
        private static IAnchorDetailFactory AnchorDetailFactory;

        [UsingDependency]
        private static IWorldFactory WorldFactory;

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

        [TestMethod]
        public void TestParentInterfaceExpansionPercentages()
        {
            //Verify assumptions
            if (AnchorDetailFactory == null)
                Assert.Inconclusive("Anchor detail factory not located.");
            if (AnchorFactory == null)
                Assert.Inconclusive("Anchor factory not located.");
            if (UserInterfaceComponentFactory == null)
                Assert.Inconclusive("User interface factory not located.");
            IWorld world = WorldFactory.CreateWorld();
            //Create a root component
            IUserInterfaceComponent expandingComponent = null;
            IUserInterfaceComponent bigComponent = null;
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0)
                ),
                createdComponent => {
                    //Add a child component which can scale but starts with no height
                    expandingComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                        world,
                        createdComponent,
                        AnchorFactory.CreateAnchor(
                            AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PERCENTAGE, 10, true),
                            AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PERCENTAGE, 10, true),
                            AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PERCENTAGE, 10, true),
                            AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PERCENTAGE, 10)
                        ),
                        subCreatedComponent => {
                            //Add a child component to that which has a huge scale
                            bigComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                                world,
                                subCreatedComponent,
                                AnchorFactory.CreateAnchor(
                                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0, true),
                                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0, true),
                                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0, true),
                                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 500, true)
                                ),
                                _ => { });
                        }
                    );
                }
            );
            parentUserInterfaceComponent.SetWidth(1000, 1000);
            //Validate the height of the expanding component
            //Minimum isn't enforced for super UI components
            Assert.AreEqual(1000, parentUserInterfaceComponent.PixelHeight, "Failed assumption");
            Assert.AreEqual(500, parentUserInterfaceComponent.MinimumPixelHeight, "Failed assumption");
            //Middle comeponnt
            Assert.AreEqual(800, expandingComponent.PixelHeight, "Failed assumption");
            Assert.AreEqual(500, expandingComponent.MinimumPixelHeight, "Failed assumption");
            //Child component
            Assert.AreEqual(500, bigComponent.PixelHeight, "Failed assumption");
            Assert.AreEqual(500, bigComponent.MinimumPixelHeight, "Failed assumption");
            //Expand the parent interface
            parentUserInterfaceComponent.SetWidth(1000, 700);
            //Verify results
            Assert.AreEqual(700, parentUserInterfaceComponent.PixelHeight);
            Assert.AreEqual(500, parentUserInterfaceComponent.MinimumPixelHeight);
            //Middle comeponnt
            Assert.AreEqual(560, expandingComponent.PixelHeight);
            Assert.AreEqual(500, expandingComponent.MinimumPixelHeight);
            //Child component
            Assert.AreEqual(500, bigComponent.PixelHeight);
            Assert.AreEqual(500, bigComponent.MinimumPixelHeight);
        }

        [TestMethod]
        public void TestParentInterfaceExpansion()
        {
            //Verify assumptions
            if (AnchorDetailFactory == null)
                Assert.Inconclusive("Anchor detail factory not located.");
            if (AnchorFactory == null)
                Assert.Inconclusive("Anchor factory not located.");
            if (UserInterfaceComponentFactory == null)
                Assert.Inconclusive("User interface factory not located.");
            IWorld world = WorldFactory.CreateWorld();
            //Create a root component
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0)
                ),
                null
            );
            parentUserInterfaceComponent.SetWidth(1000, 1000);
            //Add a child component which can scale but starts with no height
            IUserInterfaceComponent expandingComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                parentUserInterfaceComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 100, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 100, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 100, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 100)
                ),
                _ => { }
            );
            //Add a child component to that which has a huge scale
            IUserInterfaceComponent bigComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                expandingComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 500, true)
                ),
                _ => { }
            );
            //Validate the height of the expanding component
            //Minimum isn't enforced for super UI components
            Assert.AreEqual(1000, parentUserInterfaceComponent.PixelHeight, "Failed assumption");
            Assert.AreEqual(700, parentUserInterfaceComponent.MinimumPixelHeight, "Failed assumption");
            //Middle comeponnt
            Assert.AreEqual(800, expandingComponent.PixelHeight, "Failed assumption");
            Assert.AreEqual(500, expandingComponent.MinimumPixelHeight, "Failed assumption");
            //Child component
            Assert.AreEqual(500, bigComponent.PixelHeight, "Failed assumption");
            Assert.AreEqual(500, bigComponent.MinimumPixelHeight, "Failed assumption");
            //Expand the parent interface
            parentUserInterfaceComponent.SetWidth(1000, 700);
            //Verify results
            Assert.AreEqual(700, parentUserInterfaceComponent.PixelHeight);
            Assert.AreEqual(700, parentUserInterfaceComponent.MinimumPixelHeight);
            //Middle comeponnt
            Assert.AreEqual(500, expandingComponent.PixelHeight);
            Assert.AreEqual(500, expandingComponent.MinimumPixelHeight);
            //Child component
            Assert.AreEqual(500, bigComponent.PixelHeight);
            Assert.AreEqual(500, bigComponent.MinimumPixelHeight);
        }

        [TestMethod]
        public void TestExpansionWithMargin()
        {
            //Verify assumptions
            if (AnchorDetailFactory == null)
                Assert.Inconclusive("Anchor detail factory not located.");
            if (AnchorFactory == null)
                Assert.Inconclusive("Anchor factory not located.");
            if (UserInterfaceComponentFactory == null)
                Assert.Inconclusive("User interface factory not located.");
            IWorld world = WorldFactory.CreateWorld();
            //Create a root component
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0)
                ),
                null
            );
            parentUserInterfaceComponent.SetWidth(1000, 1000);
            //Add a child component which can scale but starts with no height
            IUserInterfaceComponent expandingComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                parentUserInterfaceComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 100, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 100, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 100, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 100)
                ),
                null
            );
            //Add a child component to that which has a huge scale
            IUserInterfaceComponent bigComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                expandingComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 500, true)
                ),
                null
            );
            //Validate the height of the expanding component
            //Minimum isn't enforced for super UI components
            Assert.AreEqual(1000, parentUserInterfaceComponent.PixelHeight);
            Assert.AreEqual(600, parentUserInterfaceComponent.MinimumPixelHeight);
            //Middle comeponnt
            Assert.AreEqual(500, expandingComponent.PixelHeight);
            Assert.AreEqual(500, expandingComponent.MinimumPixelHeight);
            //Child component
            Assert.AreEqual(500, bigComponent.PixelHeight);
            Assert.AreEqual(500, bigComponent.MinimumPixelHeight);
        }

        [TestMethod]
        public void TestMinimumScale()
        {
            //Verify assumptions
            if (AnchorDetailFactory == null)
                Assert.Inconclusive("Anchor detail factory not located.");
            if (AnchorFactory == null)
                Assert.Inconclusive("Anchor factory not located.");
            if (UserInterfaceComponentFactory == null)
                Assert.Inconclusive("User interface factory not located.");
            IWorld world = WorldFactory.CreateWorld();
            //Create a root component
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0)
                ),
                null
            );
            parentUserInterfaceComponent.SetWidth(1000, 1000);
            //Add a child component which can scale but starts with no height
            IUserInterfaceComponent expandingComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                parentUserInterfaceComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 100)
                ),
                null
            );
            //Add a child component to that which has a huge scale
            IUserInterfaceComponent bigComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                expandingComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 500)
                ),
                null
            );
            //Validate the height of the expanding component
            //Minimum isn't enforced for super UI components
            Assert.AreEqual(1000, parentUserInterfaceComponent.PixelHeight);
            Assert.AreEqual(500, parentUserInterfaceComponent.MinimumPixelHeight);
            //Middle comeponnt
            Assert.AreEqual(500, expandingComponent.PixelHeight);
            Assert.AreEqual(500, expandingComponent.MinimumPixelHeight);
            //Child component
            Assert.AreEqual(500, bigComponent.PixelHeight);
            Assert.AreEqual(500, bigComponent.MinimumPixelHeight);
        }

        /// <summary>
        /// Test how scaling anchors work
        /// </summary>
        [TestMethod]
        public void TestStrictExpansion()
        {
            //Verify assumptions
            if (AnchorDetailFactory == null)
                Assert.Inconclusive("Anchor detail factory not located.");
            if (AnchorFactory == null)
                Assert.Inconclusive("Anchor factory not located.");
            if (UserInterfaceComponentFactory == null)
                Assert.Inconclusive("User interface factory not located.");
            IWorld world = WorldFactory.CreateWorld();
            //Create a root component
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0)
                ),
                null
            );
            parentUserInterfaceComponent.SetWidth(1000, 1000);
            //This element should neve rexpand past 500 height
            IUserInterfaceComponent expandingComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                parentUserInterfaceComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 500, true)
                ),
                null
            );
            //Add a child component to that which has a huge scale
            IUserInterfaceComponent bigComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                expandingComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 10000)
                ),
                null
            );
            //Validate the height of the expanding component
            //Minimum isn't enforced for super UI components
            Assert.AreEqual(1000, parentUserInterfaceComponent.PixelHeight);
            Assert.AreEqual(500, parentUserInterfaceComponent.MinimumPixelHeight);
            //Middle comeponnt
            Assert.AreEqual(500, expandingComponent.PixelHeight);
            Assert.AreEqual(500, expandingComponent.MinimumPixelHeight);
            //Child component
            Assert.AreEqual(10000, bigComponent.PixelHeight);
            Assert.AreEqual(10000, bigComponent.MinimumPixelHeight);
        }

        /// <summary>
        /// Test how scaling anchors work
        /// </summary>
        [TestMethod]
        public void TestNonStrictExpansion()
        {
            //Verify assumptions
            if (AnchorDetailFactory == null)
                Assert.Inconclusive("Anchor detail factory not located.");
            if (AnchorFactory == null)
                Assert.Inconclusive("Anchor factory not located.");
            if (UserInterfaceComponentFactory == null)
                Assert.Inconclusive("User interface factory not located.");
            IWorld world = WorldFactory.CreateWorld();
            //Create a root component
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0)
                ),
                null
            );
            parentUserInterfaceComponent.SetWidth(1000, 1000);
            //Add a child component which can scale but starts with no height
            IUserInterfaceComponent expandingComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                parentUserInterfaceComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 100)
                ),
                null
            );
            //Add a child component to that which has a huge scale
            IUserInterfaceComponent bigComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                expandingComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 10000)
                ),
                null
            );
            //Validate the height of the expanding component
            //Minimum isn't enforced for super UI components
            Assert.AreEqual(1000, parentUserInterfaceComponent.PixelHeight);
            Assert.AreEqual(10000, parentUserInterfaceComponent.MinimumPixelHeight);
            //Middle comeponnt
            Assert.AreEqual(10000, expandingComponent.PixelHeight);
            Assert.AreEqual(10000, expandingComponent.MinimumPixelHeight);
            //Child component
            Assert.AreEqual(10000, bigComponent.PixelHeight);
            Assert.AreEqual(10000, bigComponent.MinimumPixelHeight);
        }

        /// <summary>
        /// Tests that a childs pixel scale is correct based on the parent size
        /// </summary>
        [DataTestMethod]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.RIGHT, AnchorUnits.PIXELS, 100, 100, 1000, 800)]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.RIGHT, AnchorUnits.PERCENTAGE, 20, 20, 1000, 600)]
        [DataRow(AnchorDirections.RIGHT, AnchorDirections.RIGHT, AnchorUnits.PERCENTAGE, 50, 20, 2000, 600)]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.LEFT, AnchorUnits.PERCENTAGE, 70, 90, 1000, 200)]
        [DataRow(AnchorDirections.LEFT, AnchorDirections.LEFT, AnchorUnits.PIXELS, 200, 500, 1000, 300)]
        [DataRow(AnchorDirections.RIGHT, AnchorDirections.RIGHT, AnchorUnits.PIXELS, 500, 100, 500, 400)]
        [DataRow(AnchorDirections.RIGHT, AnchorDirections.LEFT, AnchorUnits.PIXELS, 600, 600, 1000, 200)]
        public void TestChildScaleCorrectness(
            AnchorDirections childLeftAnchorDir,
            AnchorDirections childRightAnchorDir,
            AnchorUnits childAnchorUnits,
            double childLeftAnchorOffset,
            double childRightAnchorOffset,
            double parentWidth,
            double expectedChildPixelWidth)
        {
            //Verify assumptions
            if (AnchorDetailFactory == null)
                Assert.Inconclusive("Anchor detail factory not located.");
            if (AnchorFactory == null)
                Assert.Inconclusive("Anchor factory not located.");
            if (UserInterfaceComponentFactory == null)
                Assert.Inconclusive("User interface factory not located.");
            IWorld world = WorldFactory.CreateWorld();
            //Setup the parent component
            //Create the parent anchor details
            IAnchorDetails parentTopAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0);
            IAnchorDetails parentBottomAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0);
            IAnchorDetails parentLeftAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0);
            IAnchorDetails parentRightAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0);
            //Create the parent anchor
            IAnchor parentAnchor = AnchorFactory.CreateAnchor(parentLeftAnchorDetails, parentRightAnchorDetails, parentTopAnchorDetails, parentBottomAnchorDetails);
            //Create the parent component
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(world, parentAnchor, null);
            parentUserInterfaceComponent.SetWidth(parentWidth, 1000);
            //Setup the child component
            //Create the anchor details
            IAnchorDetails childTopAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 100);
            IAnchorDetails childBottomAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 100);
            IAnchorDetails childLeftAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(childLeftAnchorDir, childAnchorUnits, childLeftAnchorOffset);
            IAnchorDetails childRightAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(childRightAnchorDir, childAnchorUnits, childRightAnchorOffset);
            //Create the anchor
            IAnchor childAnchor = AnchorFactory.CreateAnchor(childLeftAnchorDetails, childRightAnchorDetails, childTopAnchorDetails, childBottomAnchorDetails);
            //Create the child component
            IUserInterfaceComponent childUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(world, parentUserInterfaceComponent, childAnchor, null);
            //Verify child width
            Assert.AreEqual(800, childUserInterfaceComponent.PixelHeight);
            Assert.AreEqual(expectedChildPixelWidth, childUserInterfaceComponent.PixelWidth);
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
            IWorld world = WorldFactory.CreateWorld();
            //Create the anchor details
            IAnchorDetails topAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 100);
            IAnchorDetails bottomAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 100);
            IAnchorDetails leftAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(leftAnchorDir, AnchorUnits.PIXELS, leftAnchorAmount);
            IAnchorDetails rightAnchorDetails = AnchorDetailFactory.CreateAnchorDetails(rightAnchorDir, AnchorUnits.PIXELS, rightAnchorAmount);
            //Craete the anchor
            IAnchor anchor = AnchorFactory.CreateAnchor(leftAnchorDetails, rightAnchorDetails, topAnchorDetails, bottomAnchorDetails);
            //Create a generic user interface component
            IUserInterfaceComponent userInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(world, anchor, null);
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
            IWorld world = WorldFactory.CreateWorld();
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
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(world, parentAnchor, null);
            //Create another generic user interface component
            UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(world, parentUserInterfaceComponent, childAnchor, null);
            //Check parent minimum width
            Assert.AreEqual(200, parentUserInterfaceComponent.MinimumPixelHeight);  //Should be 200 as child component requires 100 space above, and 100 space below
            Assert.AreEqual(expectedParentWidth, parentUserInterfaceComponent.MinimumPixelWidth);   //Should be 250 as child component requires 250 space left and none right.
        }

    }
}
