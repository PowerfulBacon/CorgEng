using CorgEng.UtilityTypes.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.UtilityTypes.MatrixTests
{
    /// <summary>
    /// Summary description for IdentityMatrixTests
    /// </summary>
    [TestClass]
    public class IdentityMatrixTests : TestBase
    {

        [TestMethod]
        public void TestIdentityMatrixDefinition()
        {
            //Create a generic identity matrix
            Matrix identiyMatrix = Matrix.Identity[4];
            //Check to make sure it makes sense
            for (int x = 1; x <= 4; x++)
            {
                for (int y = 1; y <= 4; y++)
                {
                    Assert.AreEqual(x == y ? 1 : 0, identiyMatrix[x, y]);
                }
            }
        }

        [TestMethod]
        public void TestIdentityMultiplication()
        {
            //Make a matrix
            Matrix knownMatrix = new Matrix(new float[,] {
                { 1, 2, 3, 4 },
                { 1, 2, 3, 4 },
                { 1, 2, 3, 4 },
                { 1, 2, 3, 4 },
            });
            //Copy that matrix
            Matrix matrixCopy = new Matrix(new float[,] {
                { 1, 2, 3, 4 },
                { 1, 2, 3, 4 },
                { 1, 2, 3, 4 },
                { 1, 2, 3, 4 },
            });
            //Make identity matrix
            Matrix identityMatrix = Matrix.Identity[4];
            //Test
            Assert.AreEqual(matrixCopy, knownMatrix * identityMatrix);
            Assert.AreEqual(matrixCopy, identityMatrix * knownMatrix);
        }

    }
}
