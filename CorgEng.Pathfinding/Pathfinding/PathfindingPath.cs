using CorgEng.GenericInterfaces.Pathfinding;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Pathfinding
{
    internal class PathfindingPath : IPath
    {

        public List<IVector<float>> Points { get; } = new List<IVector<float>>();

        public override string? ToString()
        {
            return $"{{{string.Join("},{", Points)}}}";
        }

    }
}
