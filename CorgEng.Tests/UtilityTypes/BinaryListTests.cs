using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.UtilityTypes
{
    [TestClass]
    public class BinaryListTests : TestBase
    {

        [UsingDependency]
        private static IBinaryListFactory BinaryListFactory;

        [TestMethod]
        public void TestEnumeration()
        {
            //Create a binary list
            IBinaryList<int> binaryListValues = BinaryListFactory.CreateEmpty<int>();
            //Insert some numbers
            binaryListValues.Add(6, 5);
            binaryListValues.Add(3, 2);
            binaryListValues.Add(8, 7);
            binaryListValues.Add(5, 4);
            binaryListValues.Add(1, 0);
            binaryListValues.Add(7, 6);
            binaryListValues.Add(2, 1);
            binaryListValues.Add(4, 3);
            //Check
            int i = 0;
            foreach (int value in binaryListValues)
            {
                if (value != i || i < 0 || i > 7)
                    Assert.Fail($"Failed, expected {i} but got {value}");
                i++;
            }
        }

    }
}
