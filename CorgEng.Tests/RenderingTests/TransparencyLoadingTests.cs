using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.Rendering.Textures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.RenderingTests
{
    [TestClass]
    public class TransparencyLoadingTests : TestBase
    {

        [UsingDependency]
        private static IIconFactory IconFactory;

        [TestInitialize]
        public void InitTest()
        {
            TextureCache.LoadTextureDataJson();
        }

        [TestMethod]
        [Timeout(10000)]
        public void TestTransparencyLoading()
        {
            Assert.IsTrue(IconFactory.CreateIcon("is_transparent", 0, 0).HasTransparency);
            Assert.IsTrue(IconFactory.CreateIcon("is_transparent_two", 0, 0).HasTransparency);
            Assert.IsFalse(IconFactory.CreateIcon("is_not_transparent", 0, 0).HasTransparency);
            Assert.IsFalse(IconFactory.CreateIcon("is_not_transparent_two", 0, 0).HasTransparency);
        }

    }
}
