using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Pathfinding
{
    internal class PathfindingRun
    {

        [UsingDependency]
        private static ILogger Logger;

        private Dictionary<Vector<int>, PathfinderNode> positionToNodeDictionary = new Dictionary<Vector<int>, PathfinderNode>();

        private PathfindingQueue examinationQueue = new PathfindingQueue();

        private IPathfindingRequest request;

        private Vector<int> up = new Vector<int>(0, 1);
        private Vector<int> down = new Vector<int>(0, -1);
        private Vector<int> left = new Vector<int>(-1, 0);
        private Vector<int> right = new Vector<int>(1, 0);

        public IPath? GetPath(IPathfindingRequest pathfindingRequest)
        {
            Stopwatch timer = Stopwatch.StartNew();

            request = pathfindingRequest;
            //Add the first node
            PathfinderNode initialNode = new PathfinderNode(null, (Vector<float>)request.Start, 0, (Vector<int>)request.End);
            examinationQueue.UpdateNode((Vector<float>)request.Start, 0, null, request);
            //Do processing until we can no longer process
            while (examinationQueue.HasNodes())
            {
                IPath? result = ProcessNode();
                if (result != null)
                {
                    request.OnPathFound?.Invoke(result);

                    //Temp code to test performance.
                    timer.Stop();
                    Logger.WriteLine($"Pathfinding run completed in {timer.ElapsedMilliseconds}ms, examining {positionToNodeDictionary.Count} nodes.", LogType.TEMP);

                    return result;
                }
            }
            request.OnPathFailed?.Invoke();

            //Temp code to test performance.
            timer.Stop();
            Logger.WriteLine($"Pathfinding run failed in {timer.ElapsedMilliseconds}ms, examining {positionToNodeDictionary.Count} nodes.", LogType.TEMP);

            return null;
        }

        private IPath? ProcessNode()
        {
            //Take the best node to examine
            PathfinderNode bestNode = examinationQueue.GetBest();
            //Logger.WriteLine($"Examining {bestNode.Position}. Current distance: {bestNode.SourceDistance}");
            //Check for success condition
            if (bestNode.Position.X == request.End.X && bestNode.Position.Y == request.End.Y)
            {
                return bestNode.CompilePath();
            }
            //Add surrounding nodes
            Vector<float> nodeAbove = bestNode.Position + up;
            Vector<float> nodeBelow = bestNode.Position + down;
            Vector<float> nodeLeft = bestNode.Position + left;
            Vector<float> nodeRight = bestNode.Position + right;
            int cost;
            //Above
            cost = request.PathCellQueryer.EnterPositionCost(nodeAbove, Direction.NORTH);
            if (cost != 0)
            {
                float distance = bestNode.SourceDistance + cost;
                examinationQueue.UpdateNode(nodeAbove, distance, bestNode, request);
            }
            //Below
            cost = request.PathCellQueryer.EnterPositionCost(nodeBelow, Direction.SOUTH);
            if (cost != 0)
            {
                float distance = bestNode.SourceDistance + cost;
                examinationQueue.UpdateNode(nodeBelow, distance, bestNode, request);
            }
            //Left
            cost = request.PathCellQueryer.EnterPositionCost(nodeLeft, Direction.WEST);
            if (cost != 0)
            {
                float distance = bestNode.SourceDistance + cost;
                examinationQueue.UpdateNode(nodeLeft, distance, bestNode, request);
            }
            //Right
            cost = request.PathCellQueryer.EnterPositionCost(nodeRight, Direction.EAST);
            if (cost != 0)
            {
                float distance = bestNode.SourceDistance + cost;
                examinationQueue.UpdateNode(nodeRight, distance, bestNode, request);
            }
            //No path found this time
            return null;
        }

    }
}
