using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Pathfinding
{
    internal class PathfinderExaminedNode
    {

        /// <summary>
        /// Where we came from
        /// </summary>
        public PathfinderExaminedNode? Source { get; set; }

        /// <summary>
        /// The score of this node.
        /// Higher scores indicate better paths.
        /// </summary>
        public int Score { get; set; }

    }
}
