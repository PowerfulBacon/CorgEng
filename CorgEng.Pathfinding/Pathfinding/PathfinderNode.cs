using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Pathfinding
{
    internal class PathfinderNode
    {

        /// <summary>
        /// Where we came from
        /// </summary>
        public PathfinderNode? Source { get; set; }

        /// <summary>
        /// The distance from the source
        /// </summary>
        public float SourceDistance { get; set; }

        /// <summary>
        /// How for away is this node from the end location
        /// </summary>
        public float EndDistance { get; set; }

        /// <summary>
        /// The score of this node.
        /// Higher scores indicate better paths.
        /// </summary>
        public float Score
        {
            get => SourceDistance + EndDistance;
        }

        /// <summary>
        /// The nodes position.
        /// X and Y represents the grid position of this node.
        /// All other dimension represent the variables, which expand the search space to n-dimensions
        /// </summary>
        public Vector<int> Position { get; set; }

        public PathfinderNode(PathfinderNode? source, Vector<int> position, float sourceDistance, Vector<float> destination)
        {
            Source = source;
            SourceDistance = sourceDistance;
            EndDistance = QuickDistance(position, destination);
            Position = position;
        }

        public IPath CompilePath()
        {
            PathfindingPath path = new PathfindingPath();
            CompilePath(path);
            return path;
        }

        private void CompilePath(IPath path)
        {
            Source?.CompilePath(path);
            path.Points.Add(new Vector<float>(Position.X, Position.Y));
        }



        /// <summary>
        /// The square root of two
        /// </summary>
        private static float rootTwo = (float)Math.Sqrt(2);

        /// <summary>
        /// Quickly calculate the distance between 2 vectors
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private float QuickDistance(Vector<float> start, Vector<float> end)
        {
            float xDif = Math.Abs(end.X - start.X);
            float yDif = Math.Abs(end.Y - start.Y);
            float smallDif = Math.Min(xDif, yDif);
            return smallDif * rootTwo + Math.Max(xDif, yDif) - smallDif;
        }

    }
}
