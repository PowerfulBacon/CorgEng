using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.Pathfinding.Pathfinding;
using CorgEng.UtilityTypes.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Tests.Pathfinding
{
    [TestClass]
    public class PathfindingTests
    {

        private class PathfindingTestQueryer : IPathCellQueryer
        {

            public int EnterPositionCost(IVector<float> position, Direction enterDirection)
            {
                return 1;
            }

        }

        [UsingDependency]
        private static IPathfinder Pathfinder;

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// Test a simple path
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public async Task TestSimplePath()
        {
            PathfindingRequest pathfindingRequest = new PathfindingRequest(
                new Vector<float>(0, 0),
                new Vector<float>(0, 10),
                new PathfindingTestQueryer(),
                (path) => { },
                () => {
                    Assert.Fail("Pathfinding failed");
                }
                );
            IPath foundPath = await Pathfinder.GetPath(pathfindingRequest);
            Assert.IsNotNull(foundPath);
            Logger.WriteLine(foundPath);
        }

        [TestMethod]
        [Timeout(5000)]
        public async Task TestDiagonalPath()
        {
            PathfindingRequest pathfindingRequest = new PathfindingRequest(
                new Vector<float>(0, 0),
                new Vector<float>(10, 10),
                new PathfindingTestQueryer(),
                (path) => { },
                () => {
                    Assert.Fail("Pathfinding failed");
                }
                );
            IPath foundPath = await Pathfinder.GetPath(pathfindingRequest);
            Assert.IsNotNull(foundPath);
            Logger.WriteLine(foundPath);
        }

        private class PathfindingWallTestQueryer : IPathCellQueryer
        {

            public int EnterPositionCost(IVector<float> position, Direction enterDirection)
            {
                return position.X == 5 && position.Y < 10 ? 0 : 1;
            }

        }

        [TestMethod]
        [Timeout(5000)]
        public async Task TestWallPath()
        {
            PathfindingRequest pathfindingRequest = new PathfindingRequest(
                new Vector<float>(0, 0),
                new Vector<float>(10, -10),
                new PathfindingWallTestQueryer(),
                (path) => { },
                () => {
                    Assert.Fail("Pathfinding failed");
                }
                );
            IPath foundPath = await Pathfinder.GetPath(pathfindingRequest);
            Assert.IsNotNull(foundPath);
            Logger.WriteLine(foundPath);
        }

    }
}
