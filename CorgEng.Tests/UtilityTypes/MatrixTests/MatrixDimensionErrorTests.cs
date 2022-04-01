using CorgEng.UtilityTypes.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.UtilityTypes.MatrixTests
{
    [TestClass]
    public class MatrixDimensionErrorTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidMatrixDimensionError))]
        public void TestDimensionalError()
        {
            Matrix a = new Matrix(4, 1);
            Matrix b = new Matrix(2, 4);
            Matrix c = b * a;
        }

        [TestMethod]
        public void TestValidDimensions()
        {
            Matrix a = new Matrix(4, 1);
            Matrix b = new Matrix(2, 4);
            Matrix c = a * b;
            Assert.AreEqual(2, c.X);
            Assert.AreEqual(1, c.Y);
        }
    }
}
