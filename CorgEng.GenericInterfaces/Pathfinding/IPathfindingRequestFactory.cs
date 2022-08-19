using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Pathfinding
{
    public interface IPathfindingRequestFactory
    {

        IPathfindingRequest CreateRequest(IVector<float> start, IVector<float> end, IPathCellQueryer pathCellQueryer);

        IPathfindingRequest CreateRequest(IVector<float> start, IVector<float> end, IPathCellQueryer pathCellQueryer, PathFoundDelegate onPathFound);

        IPathfindingRequest CreateRequest(IVector<float> start, IVector<float> end, IPathCellQueryer pathCellQueryer, PathFoundDelegate onPathFound, PathFailedDelegate onPathFailed);

    }
}
