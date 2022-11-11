using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes
{
    public interface ILineCaster
    {


        /// <summary>
        /// Gets all the tiles between 2 points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IEnumerable<IVector<int>> GetTilesBetween(IVector<int> start, IVector<int> end);
        IEnumerable<IVector<int>> GetTilesBetween(IVector<float> start, IVector<float> end);

    }
}
