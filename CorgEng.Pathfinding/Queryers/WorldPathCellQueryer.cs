using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.GenericInterfaces.UtilityTypes;
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

        public int EnterPositionCost(IVector<float> position, Direction enterDirection)
        {
            //TODO: Multi-z support
            if (SolidSystem.WorldLayers.ContainsKey(0))
            {
                return SolidSystem.WorldLayers[0].worldGrid.Get((int)Math.Round(position.X), (int)Math.Round(position.Y)) == null ? 1 : 0;
            }
            return 1;
        }

    }
}
