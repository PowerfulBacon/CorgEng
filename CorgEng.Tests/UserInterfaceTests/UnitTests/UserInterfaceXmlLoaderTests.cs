using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
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

        [UsingDependency]
        private static IWorldFactory WorldFactory;

        [TestMethod]
        public void TestDependencyImplementation()
        {
            Assert.IsNotNull(UserInterfaceXmlLoader);
        }

        /// <summary>
        /// !!This test is executed against the first child of the root element!!
        /// </summary>
        [DataTestMethod]
        [DataRow("UserInterfaceTests/UnitTests/Content/UserInterfaceSimple.xml", AnchorDirections.LEFT, AnchorDirections.LEFT, AnchorDirections.TOP, AnchorDirections.BOTTOM)]
        public void TestFirstCorrectSide(string filepath, AnchorDirections left, AnchorDirections right, AnchorDirections top, AnchorDirections bottom)
        {
            //Verify assumptions
            if (!File.Exists(filepath))
                Assert.Inconclusive("Specified filepath does not exist.");
            if (UserInterfaceXmlLoader == null)
                Assert.Inconclusive("User interface XML loader dependency was not injected.");
            //Check for loading errors
            IUserInterfaceComponent Root = UserInterfaceXmlLoader.LoadUserInterface(WorldFactory.CreateWorld(), filepath);
            //If the returned value if null, fail
            Assert.IsNotNull(Root, "User interface loader returned null.");
            //Execute test
            IUserInterfaceComponent firstChild = Root.GetChildren().First();
            Assert.AreEqual(left, firstChild.Anchor.LeftDetails.AnchorSide);
            Assert.AreEqual(right, firstChild.Anchor.RightDetails.AnchorSide);
            Assert.AreEqual(top, firstChild.Anchor.TopDetails.AnchorSide);
            Assert.AreEqual(bottom, firstChild.Anchor.BottomDetails.AnchorSide);
        }

        /// <summary>
        /// !!This test is executed against the first child of the root element!!
        /// </summary>
        [DataTestMethod]
        [DataRow("UserInterfaceTests/UnitTests/Content/UserInterfaceSimple.xml", false, true, false, true)]
        public void TestFirstCorrectStrictness(string filepath, bool leftStrict, bool rightStrict, bool topStrict, bool bottomStrict)
        {
            //Verify assumptions
            if (!File.Exists(filepath))
                Assert.Inconclusive("Specified filepath does not exist.");
            if (UserInterfaceXmlLoader == null)
                Assert.Inconclusive("User interface XML loader dependency was not injected.");
            //Check for loading errors
            IUserInterfaceComponent Root = UserInterfaceXmlLoader.LoadUserInterface(WorldFactory.CreateWorld(), filepath);
            //If the returned value if null, fail
            Assert.IsNotNull(Root, "User interface loader returned null.");
            //Execute test
            IUserInterfaceComponent firstChild = Root.GetChildren().First();
            Assert.AreEqual(leftStrict, firstChild.Anchor.LeftDetails.Strict);
            Assert.AreEqual(rightStrict, firstChild.Anchor.RightDetails.Strict);
            Assert.AreEqual(topStrict, firstChild.Anchor.TopDetails.Strict);
            Assert.AreEqual(bottomStrict, firstChild.Anchor.BottomDetails.Strict);
        }

        /// <summary>
        /// !!This test is executed against the first child of the root element!!
        /// </summary>
        [DataTestMethod]
        [DataRow("UserInterfaceTests/UnitTests/Content/UserInterfaceSimple.xml", AnchorUnits.PIXELS, AnchorUnits.PIXELS, AnchorUnits.PERCENTAGE, AnchorUnits.PERCENTAGE)]
        public void TestFirstCorrectAnchorUnits(string filepath, AnchorUnits leftAnchorUnits, AnchorUnits rightAnchorUnits, AnchorUnits topAnchorUnits, AnchorUnits bottomAnchorUnits)
        {
            //Verify assumptions
            if (!File.Exists(filepath))
                Assert.Inconclusive("Specified filepath does not exist.");
            if (UserInterfaceXmlLoader == null)
                Assert.Inconclusive("User interface XML loader dependency was not injected.");
            //Check for loading errors
            IUserInterfaceComponent Root = UserInterfaceXmlLoader.LoadUserInterface(WorldFactory.CreateWorld(), filepath);
            //If the returned value if null, fail
            Assert.IsNotNull(Root, "User interface loader returned null.");
            //Execute test
            IUserInterfaceComponent firstChild = Root.GetChildren().First();
            Assert.AreEqual(leftAnchorUnits, firstChild.Anchor.LeftDetails.AnchorUnits);
            Assert.AreEqual(rightAnchorUnits, firstChild.Anchor.RightDetails.AnchorUnits);
            Assert.AreEqual(topAnchorUnits, firstChild.Anchor.TopDetails.AnchorUnits);
            Assert.AreEqual(bottomAnchorUnits, firstChild.Anchor.BottomDetails.AnchorUnits);
        }

        /// <summary>
        /// !!This test is executed against the first child of the root element!!
        /// </summary>
        [DataTestMethod]
        [DataRow("UserInterfaceTests/UnitTests/Content/UserInterfaceSimple.xml", 100, 200, 10, 10)]
        public void TestFirstCorrectAnchorOffset(string filepath, double leftAnchorAmount, double rightAnchorAmount, double topAnchorAmount, double bottomAnchorAmount)
        {
            //Verify assumptions
            if (!File.Exists(filepath))
                Assert.Inconclusive("Specified filepath does not exist.");
            if (UserInterfaceXmlLoader == null)
                Assert.Inconclusive("User interface XML loader dependency was not injected.");
            //Check for loading errors
            IUserInterfaceComponent Root = UserInterfaceXmlLoader.LoadUserInterface(WorldFactory.CreateWorld(), filepath);
            //If the returned value if null, fail
            Assert.IsNotNull(Root, "User interface loader returned null.");
            //Execute test
            IUserInterfaceComponent firstChild = Root.GetChildren().First();
            Assert.AreEqual(leftAnchorAmount, firstChild.Anchor.LeftDetails.AnchorOffset);
            Assert.AreEqual(rightAnchorAmount, firstChild.Anchor.RightDetails.AnchorOffset);
            Assert.AreEqual(topAnchorAmount, firstChild.Anchor.TopDetails.AnchorOffset);
            Assert.AreEqual(bottomAnchorAmount, firstChild.Anchor.BottomDetails.AnchorOffset);
        }

        /// <summary>
        /// Test for basic errors while parsing.
        /// Doesn't validate sensible contents of the loader, just
        /// if the loader did something.
        /// </summary>
        [DataTestMethod]
        [DataRow("UserInterfaceTests/UnitTests/Content/UserInterfaceSimple.xml")]
        public void TestXmlLoadingErrors(string filepath)
        {
            //Verify assumptions
            if (!File.Exists(filepath))
                Assert.Inconclusive("Specified filepath does not exist.");
            if (UserInterfaceXmlLoader == null)
                Assert.Inconclusive("User interface XML loader dependency was not injected.");
            //Check for loading errors
            UserInterfaceXmlLoader.LoadUserInterface(WorldFactory.CreateWorld(), filepath);
        }

        [DataTestMethod]
        [DataRow("UserInterfaceTests/UnitTests/Content/UserInterfaceSimple.xml", 3, 3)]
        public void TestCorrectInterfaceComponentCount(string filepath, int expectedComponents, int expectedDepth)
        {
            //Verify assumptions
            if (!File.Exists(filepath))
                Assert.Inconclusive("Specified filepath does not exist.");
            if (UserInterfaceXmlLoader == null)
                Assert.Inconclusive("User interface XML loader dependency was not injected.");
            //Check for loading errors
            IUserInterfaceComponent Root = UserInterfaceXmlLoader.LoadUserInterface(WorldFactory.CreateWorld(), filepath);
            //If the returned value if null, fail
            Assert.IsNotNull(Root, "User interface loader returned null.");
            //Validate assumptions
            int maxDepth = GetMaxDepth(Root);
            int componentCount = CountComponents(Root);
            //Assertions
            Assert.AreEqual(expectedDepth, maxDepth);
            Assert.AreEqual(expectedComponents, componentCount);
        }

        /// <summary>
        /// Count the number of components in a user interface
        /// </summary>
        private int CountComponents(IUserInterfaceComponent component)
        {
            //Return max of all children
            int number = 1;
            foreach (IUserInterfaceComponent child in component.GetChildren())
                number += CountComponents(child);
            return number;
        }

        /// <summary>
        /// Get the maximum depth of a user interface
        /// </summary>
        private int GetMaxDepth(IUserInterfaceComponent component, int depth = 1)
        {
            //No children, return depth
            if (component.GetChildren().Count == 0)
                return depth;
            //Return max of all children
            int greatest = 0;
            foreach (IUserInterfaceComponent child in component.GetChildren())
            {
                greatest = Math.Max(greatest, GetMaxDepth(child, depth + 1));
            }
            return greatest;
        }

    }
}
