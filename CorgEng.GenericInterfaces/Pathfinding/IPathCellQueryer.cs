using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Pathfinding
{
    /// <summary>
    /// Used to determine whether or not we can enter
    /// a node during pathfinding
    /// </summary>
    public interface IPathCellQueryer
    {

        /// <summary>
        /// Can we enter a specific cell.
        /// </summary>
        /// <param name="position">The position that is trying to be entered.</param>
        /// <param name="enterDirection">The directino the cell is being entered from. If a node attempts to travel from below into this cell, it will be north.</param>
        /// <returns></returns>
        bool CanEnterPosition(IVector<float> position, Direction enterDirection);

    }
}
