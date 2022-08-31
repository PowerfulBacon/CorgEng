using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Pathfinding
{
    public interface IPathfindingRequest
    {

        IVector<float> Start { get; }

        IVector<int> End { get; }

        /// <summary>
        /// Lets us query if we can enter a specified cell.
        /// </summary>
        IPathCellQueryer PathCellQueryer { get; }

        PathFoundDelegate OnPathFound { get; }

        PathFailedDelegate OnPathFailed { get; }

    }
}
