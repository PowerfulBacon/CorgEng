using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Pathfinding
{

    public class PathfindingRequest : IPathfindingRequest
    {
        public IVector<float> Start { get; set; }

        public IVector<int> End { get; set; }

        public IPathCellQueryer PathCellQueryer { get; set; }

        public PathFoundDelegate OnPathFound { get; set; }

        public PathFailedDelegate OnPathFailed { get; set; }

        public PathfindingRequest(IVector<float> start, IVector<int> end, IPathCellQueryer pathCellQueryer)
        {
            Start = start;
            End = end;
            PathCellQueryer = pathCellQueryer;
        }

        public PathfindingRequest(IVector<float> start, IVector<int> end, IPathCellQueryer pathCellQueryer, PathFoundDelegate onPathFound)
        {
            Start = start;
            End = end;
            PathCellQueryer = pathCellQueryer;
            OnPathFound = onPathFound;
        }

        public PathfindingRequest(IVector<float> start, IVector<int> end, IPathCellQueryer pathCellQueryer, PathFoundDelegate onPathFound, PathFailedDelegate onPathFailed)
        {
            Start = start;
            End = end;
            PathCellQueryer = pathCellQueryer;
            OnPathFound = onPathFound;
            OnPathFailed = onPathFailed;
        }
    }
}
