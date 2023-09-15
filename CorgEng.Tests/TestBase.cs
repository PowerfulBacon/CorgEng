using CorgEng.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests
{
    [TestClass]
    public class TestBase
    {
        [TestCleanup]
        public void TestCleanup()
        {
            CorgEngMain.Cleanup();
        }
    }
}
