using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.Depreciated.Managers
{
    [Obsolete]
    public static class PhysicsManager
    {

        [UsingDependency]
        public static IBinaryListFactory BinaryListFactory = default!;

        private static IBinaryList<PhysicsMap>? physicMaps = null;

        /// <summary>
        /// Get the physics map at the specific map layer.
        /// Returns null if physics is not initialised on that level.
        /// </summary>
        /// <param name="map_layer"></param>
        /// <returns></returns>
        public static PhysicsMap GetLevel(int map_layer)
        {
            // Create physics map store if necessary
            if (physicMaps == null)
                physicMaps = BinaryListFactory.CreateEmpty<PhysicsMap>();
            PhysicsMap locatedMap = physicMaps.ElementWithKey(map_layer);
            if (locatedMap != null)
            {
                return locatedMap;
            }
            // Otherwise, make a new map
            locatedMap = new PhysicsMap();
            physicMaps.Add(map_layer, locatedMap);
            return locatedMap;
        }

    }
}
