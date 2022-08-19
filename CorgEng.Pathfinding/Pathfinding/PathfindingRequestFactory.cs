using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Pathfinding
{
    [Dependency]
    internal class PathfindingRequestFactory : IPathfindingRequestFactory
    {

        public IPathfindingRequest CreateRequest(IVector<float> start, IVector<float> end, IPathCellQueryer pathCellQueryer)
        {
            return new PathfindingRequest(start, end, pathCellQueryer);
        }

        public IPathfindingRequest CreateRequest(IVector<float> start, IVector<float> end, IPathCellQueryer pathCellQueryer, PathFoundDelegate onPathFound)
        {
            return new PathfindingRequest(start, end, pathCellQueryer, onPathFound);     
        }

        public IPathfindingRequest CreateRequest(IVector<float> start, IVector<float> end, IPathCellQueryer pathCellQueryer, PathFoundDelegate onPathFound, PathFailedDelegate onPathFailed)
        {
            return new PathfindingRequest(start, end, pathCellQueryer, onPathFound, onPathFailed);
        }

    }
}
