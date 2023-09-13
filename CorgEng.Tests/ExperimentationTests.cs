using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Tests
{
    /// <summary>
    /// Class for experimenting with C# features
    /// </summary>
    [TestClass]
    public class ExperimentationTests : TestBase
    {

        [TestMethod]
        [Timeout(1000)]
        public void TestAutoWaitHandler()
        {
            for (int i = 0; i < 100; i++)
            {
                AutoResetEvent synchronousWaitEvent = new AutoResetEvent(false);
                synchronousWaitEvent.Set();
                synchronousWaitEvent.WaitOne();
            }
        }

    }
}
