using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using CorgEng.GenericInterfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.World.WorldTracking
{
    [Dependency]
    internal class World : IWorld
    {

        [UsingDependency]
        private static IPositionBasedBinaryListFactory PositionBasedBinaryListFactory;

        [UsingDependency]
        private static IBinaryListFactory BinaryListFactory;

        private IDictionary<string, IBinaryList<IPositionBasedBinaryList<ContentsHolder>>> _worldtiles;

        private IDictionary<string, IBinaryList<IPositionBasedBinaryList<ContentsHolder>>> WorldTiles
        {
            get
            {
                if (_worldtiles == null)
                    _worldtiles = new Dictionary<string, IBinaryList<IPositionBasedBinaryList<ContentsHolder>>>();
                return _worldtiles;
            }
        }

        public void AddEntity(string trackKey, IEntity entity, double x, double y, int mapLevel)
        {
            if (entity.ContentsIndex != -1)
                throw new Exception($"Attempting to insert an entity while it is already in another location");
            if (!WorldTiles.ContainsKey(trackKey))
            {
                WorldTiles.Add(trackKey, BinaryListFactory.CreateEmpty<IPositionBasedBinaryList<ContentsHolder>>());
            }
            IPositionBasedBinaryList<ContentsHolder> targetLevel = WorldTiles[trackKey].ElementWithKey(mapLevel);
            //Get the z-level to affect
            if (targetLevel == null)
            {
                targetLevel = PositionBasedBinaryListFactory.CreateEmpty<ContentsHolder>();
                WorldTiles[trackKey].Add(mapLevel, targetLevel);
            }
            //Get the position to affect
            int xInteger = (int)Math.Floor(x);
            int yInteger = (int)Math.Floor(y);
            //Add the entity
            ContentsHolder worldTile = targetLevel.Get(xInteger, yInteger);
            if (worldTile == null)
            {
                worldTile = new ContentsHolder();
                targetLevel.Add(xInteger, yInteger, worldTile);
            }
            worldTile.Insert(entity);
        }

        public void AddEntity(IEntity entity, double x, double y, int mapLevel)
        {
            AddEntity("_world", entity, x, y, mapLevel);
        }

        public IContentsHolder GetContentsAt(string trackKey, double x, double y, int mapLevel)
        {
            if (!WorldTiles.ContainsKey(trackKey))
                return null;
            return WorldTiles[trackKey].ElementWithKey(mapLevel)?.Get((int)Math.Floor(x), (int)Math.Floor(y));
        }

        public IContentsHolder GetContentsAt(double x, double y, int mapLevel)
        {
            return GetContentsAt("_world", x, y, mapLevel);
        }

        public void RemoveEntity(string trackKey, IEntity entity, double x, double y, int mapLevel)
        {
            if (entity.ContentsIndex == -1)
                throw new Exception($"Attempting to remove an entity from an invalid location ({x}, {y})");
            IPositionBasedBinaryList<ContentsHolder> targetLevel = WorldTiles[trackKey].ElementWithKey(mapLevel);
            //Target doesn't exist
            if (targetLevel == null)
                return;
            //Find the tile
            //Get the position to affect
            int xInteger = (int)Math.Floor(x);
            int yInteger = (int)Math.Floor(y);
            //Add the entity
            ContentsHolder worldTile = targetLevel.Get(xInteger, yInteger);
            if (worldTile == null)
                return;
            worldTile.Remove(entity);
        }

        public void RemoveEntity(IEntity entity, double x, double y, int mapLevel)
        {
            RemoveEntity("_world", entity, x, y, mapLevel);
        }
    }
}
