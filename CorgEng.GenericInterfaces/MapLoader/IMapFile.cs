using CorgEng.GenericInterfaces.ContentLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.MapLoader
{
    public interface IMapFile
    {

        /// <summary>
        /// The width of the map file
        /// </summary>
        int MapWidth { get; }

        /// <summary>
        /// The height of the map file
        /// </summary>
        int MapHeight { get; }

        /// <summary>
        /// The contents of the map file.
        /// Array of X and Y, with a list of all entities at that location.
        /// </summary>
        List<IEntityDefinition>[,] MapContents { get; }

    }
}
