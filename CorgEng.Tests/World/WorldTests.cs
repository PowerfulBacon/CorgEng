using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.World;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.World
{
    [TestClass]
    public class WorldTests
    {

        [UsingDependency]
        private static IWorld WorldAccess;

        [TestMethod]
        public void TestWorld()
        {
            
        }

    }
}
