using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Font.Characters;
using CorgEng.GenericInterfaces.Font.Fonts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.FontTests
{
    [TestClass]
    public class FontTests
    {

        [UsingDependency]
        private static IFontFactory FontFactory;

        [TestMethod]
        public void TestLoadingSampleFont()
        {
            IFont defaultFont = FontFactory.GetFont("CourierCode");
            //Test a few known characters
            IFontCharacter knownCharacter = defaultFont.GetCharacter(32);
            Assert.AreEqual(32, knownCharacter.CharacterCode, "Character code should be 32");
            Assert.AreEqual(2023, knownCharacter.TextureXPosition, "X code should be 2023");
            Assert.AreEqual(0, knownCharacter.TextureYPosition, "Y code should be 0");
            Assert.AreEqual(6, knownCharacter.TextureWidth, "Width code should be 6");
            Assert.AreEqual(1, knownCharacter.TextureHeight, "Height code should be 1");
            Assert.AreEqual(-2, knownCharacter.CharacterXOffset, "X Offset code should be -2");
            Assert.AreEqual(468, knownCharacter.CharacterYOffset, "Y Offset code should be 468");
            Assert.AreEqual(244, knownCharacter.CharacterXAdvance, "X Advance code should be 244");
        }

        [TestMethod]
        public void TestLoadingSampleFontErrors()
        {
            FontFactory.GetFont("CourierCode");
        }

        [TestMethod]
        public void TestDependencyImplementation()
        {
            Assert.IsNotNull(FontFactory);
        }

    }
}
