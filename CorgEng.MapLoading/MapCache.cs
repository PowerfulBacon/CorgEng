using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.MapLoading
{
    internal static class MapCache
    {

        /// <summary>
        /// A dictionary of loaded maps.
        /// </summary>
        public static Dictionary<string, MapFile> LoadedMaps { get; } = new Dictionary<string, MapFile>();

    }
}
