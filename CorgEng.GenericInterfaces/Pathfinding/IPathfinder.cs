using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Pathfinding
{
    public interface IPathfinder
    {

        /// <summary>
        /// Locate the path between the start and the end node
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="onPathFound"></param>
        /// <param name="onPathFailed"></param>
        /// <returns></returns>
        Task<IPath> GetPath(IPathfindingRequest pathfindingRequest);

    }
}
