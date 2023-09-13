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
    public class MultiplicationMatrixTests : TestBase
    {

        [TestMethod]
        public void TestSquaring()
        {

            //First matrix
            Matrix m1 = new Matrix(new float[,] {
                { 1, 2 },
                { 3, 4 }
            });

            //Second matrix
            Matrix m2 = new Matrix(new float[,] {
                { 1, 2 },
                { 3, 4 }
            });

            Matrix result = new Matrix(new float[,] {
                { 1 * 1 + 2 * 3, 1 * 2 + 2 * 4 },
                { 3 * 1 + 4 * 3, 3 * 2 + 4 * 4 }
            });

            Assert.AreEqual(result, m1 * m2);
        }

        [TestMethod]
        public void TestDifferentMatrixMultiplication()
        {

            //First matrix
            Matrix m1 = new Matrix(new float[,] {
                { 1, 2 },
                { 3, 4 }
            });

            //Second matrix
            Matrix m2 = new Matrix(new float[,] {
                { 4, 2 },
                { 3, 1 }
            });

            Matrix result = new Matrix(new float[,] {
                { 10, 4 },
                { 24, 10 }
            });

            Assert.AreEqual(result, m1 * m2);
        }

        [TestMethod]
        public void Test4dMatrixMultiplication()
        {
            //First matrix
            Matrix m1 = new Matrix(new float[,] {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 },
            });

            //Second matrix
            Matrix m2 = new Matrix(new float[,] {
                { 9, 9, 9, 6 },
                { 9, 6, 9, 9 },
                { 1, 4, 2, 3 },
                { 8, 9, 6, 4 }
            });

            Matrix result = new Matrix(new float[,] {
                { 62, 69, 57, 49 },
                { 170, 181, 161, 137 },
                { 278, 293, 265, 225 },
                { 386, 405, 369, 313 }
            });

            Assert.AreEqual(result, m1 * m2);
        }

    }
}
