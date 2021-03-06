using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.World
{
    public interface IWorld
    {

        /// <summary>
        /// Adds an entity to the world tracking
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mapLevel"></param>
        void AddEntity(IEntity entity, double x, double y, int mapLevel);

        /// <summary>
        /// Removes an entity from world tracking
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mapLevel"></param>
        void RemoveEntity(IEntity entity, double x, double y, int mapLevel);

        /// <summary>
        /// Gets all of the contents at a given location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mapLevel"></param>
        /// <returns></returns>
        IContentsHolder GetContentsAt(double x, double y, int mapLevel);

    }
}
