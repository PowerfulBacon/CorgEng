using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.MapLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.MapLoading
{
    [Dependency]
    public class MapLoader : IMapLoader
    {

        /// <summary>
        /// Loads a specified map file from the location.
        /// </summary>
        /// <param name="fileLocation">The location of the map file.</param>
        /// <returns>Return an IMapFile, containing information about the loaded map</returns>
        public IMapFile LoadMap(string fileLocation)
        {
            //Is the map already loaded?
            if (MapCache.LoadedMaps.ContainsKey(fileLocation))
                return MapCache.LoadedMaps[fileLocation];
            //Load the map file from a file
            MapFile loadedMapFile = new MapFile();
            throw new System.NotImplementedException();
        }

    }
}
