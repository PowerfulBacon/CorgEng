using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
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
        /// Translate a world position into its respective grid position.
        /// >0 && <=1 -> 0
        /// >1 && <=2 -> 1
        /// etc.
        /// </summary>
        /// <param name="sourcePosition">The input position to translate to grid coordinates.</param>
        /// <returns>Returns the grid coordinates of the provided source position.</returns>
        IVector<int> GetGridPosition(IVector<float> sourcePosition);

        /// <summary>
        /// Adds an entity to the world tracking
        /// </summary>
        /// <param name="trackKey">The key of the map level to use. Allows for optimised searching of specific items that are searched a lot.</param>
        /// <param name="entity"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mapLevel"></param>
        void AddEntity(IWorldTrackComponent trackComponent, double x, double y, int mapLevel);

        /// <summary>
        /// Removes an entity from world tracking
        /// </summary>
        /// <param name="trackKey">The key of the map level to use. Allows for optimised searching of specific items that are searched a lot.</param>
        /// <param name="entity"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mapLevel"></param>
        void RemoveEntity(IWorldTrackComponent trackComponent, double x, double y, int mapLevel);

        /// <summary>
        /// Gets all of the contents at a given location.
        /// </summary>
        /// <param name="trackKey">The key of the map level to use. Allows for optimised searching of specific items that are searched a lot.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mapLevel"></param>
        /// <returns></returns>
        IContentsHolder GetContentsAt(double x, double y, int mapLevel);

        /// <summary>
        /// Adds an entity to the world tracking
        /// </summary>
        /// <param name="trackKey">The key of the map level to use. Allows for optimised searching of specific items that are searched a lot.</param>
        /// <param name="trackComponent"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mapLevel"></param>
        void AddEntity(string trackKey, IWorldTrackComponent trackComponent, double x, double y, int mapLevel);

        /// <summary>
        /// Removes an entity from world tracking
        /// </summary>
        /// <param name="trackKey">The key of the map level to use. Allows for optimised searching of specific items that are searched a lot.</param>
        /// <param name="trackComponent"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mapLevel"></param>
        void RemoveEntity(string trackKey, IWorldTrackComponent trackComponent, double x, double y, int mapLevel);

        /// <summary>
        /// Gets all of the contents at a given location.
        /// </summary>
        /// <param name="trackKey">The key of the map level to use. Allows for optimised searching of specific items that are searched a lot.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mapLevel"></param>
        /// <returns></returns>
        IContentsHolder GetContentsAt(string trackKey, double x, double y, int mapLevel);

        /// <summary>
        /// Gets the entire default map level contents.
        /// This allows the position based binary list to be queried directly, for example
        /// if large portions of the world need to be accessed.
        /// </summary>
        /// <param name="mapLevel">The map level to get from</param>
        /// <returns></returns>
        IPositionBasedBinaryList<IContentsHolder> GetContents(int mapLevel);

        /// <summary>
        /// Gets the entire map level contents.
        /// This allows the position based binary list to be queried directly, for example
        /// if large portions of the world need to be accessed.
        /// </summary>
        /// <param name="trackKey">The track key to gather.</param>
        /// <param name="mapLevel">The map level to get from</param>
        /// <returns></returns>
        IPositionBasedBinaryList<IContentsHolder> GetContents(string trackKey, int mapLevel);

    }
}
