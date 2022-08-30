using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Pathfinding
{
    internal class PathfindingQueue
    {

        private Dictionary<Vector<int>, PathfinderNode> nodeLookup = new Dictionary<Vector<int>, PathfinderNode>();

        private SortedDictionary<float, List<Vector<int>>> nodeQueue = new SortedDictionary<float, List<Vector<int>>>();

        /// <summary>
        /// Get the best node that needs processing
        /// </summary>
        /// <returns></returns>
        public PathfinderNode GetBest()
        {
            float firstKey = nodeQueue.First().Key;
            List<Vector<int>> firstList = nodeQueue[firstKey];
            Vector<int> firstPosition = firstList[0];
            firstList.RemoveAt(0);
            if (firstList.Count == 0)
            {
                nodeQueue.Remove(firstKey);
            }
            return nodeLookup[firstPosition];
        }

        /// <summary>
        /// Do we have any nodes left to examine>
        /// </summary>
        /// <returns></returns>
        public bool HasNodes()
        {
            return nodeQueue.Count > 0;
        }

        /// <summary>
        /// Update the node at the position provided, or add it if it doesn't exist
        /// </summary>
        /// <param name="position"></param>
        /// <param name="newDistance"></param>
        /// <param name="newSource"></param>
        public void UpdateNode(Vector<int> position, float newDistance, PathfinderNode? newSource, IPathfindingRequest request)
        {
            if (nodeLookup.ContainsKey(position))
            {
                //Get the node to update
                PathfinderNode toUpdate = nodeLookup[position];
                //Check if its better
                if (toUpdate.SourceDistance <= newDistance)
                    return;
                //Remove the node from its old position in the queue
                nodeQueue[toUpdate.Score].Remove(position);
                if (nodeQueue[toUpdate.Score].Count == 0)
                {
                    nodeQueue.Remove(toUpdate.Score);
                }
                //Update the node
                toUpdate.SourceDistance = newDistance;
                toUpdate.Source = newSource;
                //Add the node to the new position in the queue
                if (!nodeQueue.ContainsKey(toUpdate.Score))
                {
                    nodeQueue.Add(toUpdate.Score, new List<Vector<int>>()
                    {
                        position,
                    });
                }
                else
                {
                    nodeQueue[toUpdate.Score].Add(position);
                }
            }
            else
            {
                //A new node was created
                PathfinderNode createdNode = new PathfinderNode(newSource, position, newDistance, (Vector<int>)request.End);
                //Add the reference to this node
                nodeLookup.Add(position, createdNode);
                //Add the node to the new position in the queue
                if (!nodeQueue.ContainsKey(createdNode.Score))
                {
                    nodeQueue.Add(createdNode.Score, new List<Vector<int>>()
                    {
                        position,
                    });
                }
                else
                {
                    nodeQueue[createdNode.Score].Add(position);
                }
            }
        }

    }
}
