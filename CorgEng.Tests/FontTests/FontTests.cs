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
    public class FontTests : TestBase
    {

        [UsingDependency]
        private static IFontFactory FontFactory;

        [TestMethod]
        public void TestLoadingSampleFont()
        {
            IFont defaultFont = FontFactory.GetFont("CourierCode");
            //Test a few known characters (First)
            IFontCharacter knownCharacter = defaultFont.GetCharacter(32);
            Assert.AreEqual(32, knownCharacter.CharacterCode, "Character code should be 32");
            Assert.AreEqual(2023, knownCharacter.TextureXPosition, "X code should be 2023");
            Assert.AreEqual(0, knownCharacter.TextureYPosition, "Y code should be 0");
            Assert.AreEqual(6, knownCharacter.TextureWidth, "Width code should be 6");
            Assert.AreEqual(1, knownCharacter.TextureHeight, "Height code should be 1");
            Assert.AreEqual(-2, knownCharacter.CharacterXOffset, "X Offset code should be -2");
            Assert.AreEqual(468, knownCharacter.CharacterYOffset, "Y Offset code should be 468");
            Assert.AreEqual(244, knownCharacter.CharacterXAdvance, "X Advance code should be 244");
            //Test the last character
            IFontCharacter lastCharacter = defaultFont.GetCharacter(126);
            Assert.AreEqual(126, lastCharacter.CharacterCode, "Character code should be 126");
            Assert.AreEqual(622, lastCharacter.TextureXPosition, "X code should be 622");
            Assert.AreEqual(1563, lastCharacter.TextureYPosition, "Y code should be 1563");
            Assert.AreEqual(191, lastCharacter.TextureWidth, "Width code should be 191");
            Assert.AreEqual(46, lastCharacter.TextureHeight, "Height code should be 46");
            Assert.AreEqual(27, lastCharacter.CharacterXOffset, "X Offset code should be 27");
            Assert.AreEqual(201, lastCharacter.CharacterYOffset, "Y Offset code should be 201");
            Assert.AreEqual(244, lastCharacter.CharacterXAdvance, "X Advance code should be 244");
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
