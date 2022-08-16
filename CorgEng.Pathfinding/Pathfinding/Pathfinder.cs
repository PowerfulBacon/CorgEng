using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Pathfinding
{
    [Dependency]
    internal class Pathfinder : IPathfinder
    {
        public async Task<IPath> GetPath(IPathfindingRequest pathfindingRequest)
        {
            //Perform pathfinding on an awaitable thread
            return await Task.Run(() => {
                PathfindingRun currentRun = new PathfindingRun();
                return currentRun.GetPath(pathfindingRequest);
            });
        }

    }
}
