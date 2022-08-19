using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Core
{
    public enum Direction
    {
        NORTH = 1,
        EAST = 2,
        SOUTH = 4,
        WEST = 8,
        NORTH_EAST = NORTH | EAST,
        SOUTH_EAST = SOUTH | EAST,
        SOUTH_WEST = SOUTH | WEST,
        NORTH_WEST = NORTH | WEST,
    }
}
