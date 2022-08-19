using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Pathfinding
{
    public interface IPath
    {

        /// <summary>
        /// The points on the calculated path.
        /// </summary>
        public List<IVector<float>> Points { get; }

    }
}
