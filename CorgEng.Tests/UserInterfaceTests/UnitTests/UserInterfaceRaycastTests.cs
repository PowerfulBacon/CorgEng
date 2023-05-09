using CorgEng.Core.Dependencies;
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
    public class UserInterfaceRaycastTests : TestBase
    {

        [UsingDependency]
        private static IUserInterfaceComponentFactory UserInterfaceComponentFactory;

        [UsingDependency]
        private static IAnchorFactory AnchorFactory;

        [UsingDependency]
        private static IAnchorDetailFactory AnchorDetailFactory;

        [UsingDependency]
        private static IWorldFactory WorldFactory;

        [DataTestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 50, 50)]
        [DataRow(1, 500, 200)]
        [DataRow(2, 500, 500)]
        [DataRow(0, 950, 950)]
        [DataRow(2, 100, 400)]
        [DataRow(2, 900, 900)]
        public void TestRaycast(int target, int x, int y)
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
            //(0, 0) -> (1000, 1000)
            IUserInterfaceComponent parentUserInterfaceComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                null,
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
            //(100, 100) -> (900, 900)
            IUserInterfaceComponent expandingComponent = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                world,
                parentUserInterfaceComponent,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PERCENTAGE, 10, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PERCENTAGE, 10, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PERCENTAGE, 10, true),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PERCENTAGE, 10)
                ),
                null
            );
            //Add a child component to that which has a huge scale
            //(100, 400) -> (900, 900)
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
            //Rayquery the top level component
            switch (target)
            {
                case 0:
                    Assert.AreEqual(parentUserInterfaceComponent, parentUserInterfaceComponent.Screencast(x, y));
                    break;
                case 1:
                    Assert.AreEqual(expandingComponent, parentUserInterfaceComponent.Screencast(x, y));
                    break;
                case 2:
                    Assert.AreEqual(bigComponent, parentUserInterfaceComponent.Screencast(x, y));
                    break;
                default:
                    Assert.Fail("Bad test params.");
                    return;
            }
        }

    }
}
