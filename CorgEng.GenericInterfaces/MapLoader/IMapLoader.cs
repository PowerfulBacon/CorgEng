using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.MapLoader
{
    public interface IMapLoader
    {

        /// <summary>
        /// Loads a specified map file from the location.
        /// </summary>
        /// <param name="fileLocation">The location of the map file.</param>
        /// <returns>Return an IMapFile, containing information about the loaded map</returns>
        IMapFile LoadMap(string fileLocation);

    }
}
