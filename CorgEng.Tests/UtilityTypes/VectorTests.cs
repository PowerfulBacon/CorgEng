using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.UtilityTypes
{
    [TestClass]
    public class VectorTests
    {

        [TestMethod]
        public void TestVectorLength()
        {
            IVector<float> vector = new Vector<float>(3, 4, 0);
            Assert.AreEqual(5, vector.Length());
        }

        [TestMethod]
        public void TestVectorEquivilance()
        {
            IVector<float> a = new Vector<float>(1, 2, 3);
            IVector<float> b = new Vector<float>(1, 2, 3);
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void TestVectorIndependance()
        {
            IVector<float> a = new Vector<float>(1, 2, 3);
            IVector<float> b = a.Copy();
            a[1] = 4;
            Assert.AreEqual(2, b[1]);
        }

        [TestMethod]
        public void TestVectorIndependanceAgain()
        {
            IVector<float> a = new Vector<float>(1, 2, 3);
            IVector<float> b = a.Copy();
            a = new Vector<float>(10, 10, 10);
            Assert.AreEqual(new Vector<float>(1, 2, 3), b);
            Assert.AreEqual(new Vector<float>(10, 10, 10), a);
        }

        [TestMethod]
        public void TestDictionaryKey()
        {
            IVector<float> a = new Vector<float>(1, 1, 1);
            Dictionary<IVector<float>, bool> testDict = new Dictionary<IVector<float>, bool>();
            testDict.Add(a, true);
            Assert.IsTrue(testDict.ContainsKey(a), "Could not located same struct");
            Assert.IsTrue(testDict.ContainsKey(new Vector<float>(1, 1, 1)), "Could not locate identical struct");
            Assert.IsTrue(testDict.ContainsKey(new Vector<float>((int)1.4, 1, 1)), "Could not locate integer converted struct");
            Assert.IsFalse(testDict.ContainsKey(new Vector<float>(2, 1, 1)), "Hash collisions detected");
            Assert.IsFalse(a.GetHashCode() == new Vector<float>(2, 1, 1).GetHashCode(), "Hash collision detected.");
        }

        /// <summary>
        /// Tests the dot product of A with B
        /// (1, 1, 1) dot (2, 2, 2) = 2 + 2 + 2 = 6
        /// </summary>
        [TestMethod]
        public void TestVectorDotProduct()
        {
            IVector<float> a = new Vector<float>(1, 1, 1);
            IVector<float> b = new Vector<float>(2, 2, 2);
            Assert.AreEqual(6, Vector<float>.DotProduct(a, b));
        }

        /// <summary>
        /// Test multiplication of vectors.
        ///  - Multiplying 2 vectors
        ///  - Multiplying vector by an integer scalar
        ///  - Multiplying vector by a floating point scalar
        /// </summary>
        [TestMethod]
        public void TestVectorMultiplication()
        {
            Vector<float> a = new Vector<float>(1, 1, 1);
            Vector<float> b = new Vector<float>(2, 2, 2);
            Assert.AreEqual(new Vector<float>(2, 2, 2), a * b);
            Assert.AreEqual(new Vector<float>(2, 2, 2), a * 2);
            Assert.AreEqual(new Vector<float>(5, 5, 5), b * 2.5f);
        }

        [TestMethod]
        public void TestVectorAddition()
        {
            Vector<float> a = new Vector<float>(1, 1, 1);
            Vector<float> b = new Vector<float>(2, 2, 2);
            Assert.AreEqual(new Vector<float>(3, 3, 3), a + b);
        }

    }
}
