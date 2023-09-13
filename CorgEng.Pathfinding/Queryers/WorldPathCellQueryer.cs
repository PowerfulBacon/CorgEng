using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.World;
using CorgEng.Pathfinding.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Queryers
{
    public class WorldPathCellQueryer : IPathCellQueryer
    {

        [UsingDependency]
        private static IEntityPositionTracker WorldAccess;

        public int EnterPositionCost(IVector<float> position, Direction enterDirection)
        {
            //TODO: Multi-z support
            if (SolidSystem.WorldLayers.ContainsKey(0))
            {
                IVector<int> gridPosition = WorldAccess.GetGridPosition(position);
                return SolidSystem.WorldLayers[0].worldGrid.Get(gridPosition.X, gridPosition.Y) == null ? 1 : 0;
            }
            return 1;
        }

    }
}
