using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Pathfinding.Pathfinding
{
    internal class WorldGrid
    {

        [UsingDependency]
        private static IPositionBasedBinaryListFactory PositionBasedBinaryListFactory;

        /// <summary>
        /// The world grid with the number being the amount of solid tiles
        /// </summary>
        public IPositionBasedBinaryList<int?> worldGrid;

        public WorldGrid()
        {
            worldGrid = PositionBasedBinaryListFactory.CreateEmpty<int?>();
        }

        public void AddElement(int x, int y)
        {
            int? valueAtLocation = worldGrid.Get(x, y);
            if (valueAtLocation != null)
            {
                worldGrid.Set(x, y, valueAtLocation + 1);
            }
            else
            {
                worldGrid.Add(x, y, 1);
            }
        }

        public void RemoveElement(int x, int y)
        {
            int? valueAtLocation = worldGrid.Get(x, y);
            if (valueAtLocation - 1 == 0)
            {
                worldGrid.Remove(x, y);
            }
            else
            {
                worldGrid.Set(x, y, valueAtLocation - 1);
            }
        }

    }
}
