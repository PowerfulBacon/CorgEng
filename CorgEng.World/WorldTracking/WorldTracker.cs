using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using CorgEng.UtilityTypes.PositionBasedBinaryLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.World.WorldTracking
{
    internal class WorldTracker
    {

        [UsingDependency]
        private static IPositionBasedBinaryListFactory PositionBasedBinaryListFactory;

        [UsingDependency]
        private static IBinaryListFactory BinaryListFactory;

        private IBinaryList<IPositionBasedBinaryList<WorldTile>> _worldtiles;

        private IBinaryList<IPositionBasedBinaryList<WorldTile>> WorldTiles
        {
            get
            {
                if (_worldtiles == null)
                    _worldtiles = BinaryListFactory.CreateEmpty<IPositionBasedBinaryList<WorldTile>>();
                return _worldtiles;
            }
        }

        public void AddEntity(Entity entity, double x, double y, int mapLevel)
        {
            IPositionBasedBinaryList<WorldTile> targetLevel = WorldTiles.ElementWithKey(mapLevel);
            //Get the z-level to affect
            if (targetLevel == null)
            {
                targetLevel = PositionBasedBinaryListFactory.CreateEmpty<WorldTile>();
                WorldTiles.Add(mapLevel, targetLevel);
            }
            //Get the position to affect
            int xInteger = (int)Math.Floor(x);
            int yInteger = (int)Math.Floor(y);
            //Add the entity
            WorldTile worldTile = targetLevel.Get(xInteger, yInteger);
            if (worldTile == null)
            {
                worldTile = new WorldTile();
                targetLevel.Add(xInteger, yInteger, worldTile);
            }
            worldTile.Insert(entity);
        }

        public void RemoveEntity(Entity entity, double x, double y, int mapLevel)
        {
            IPositionBasedBinaryList<WorldTile> targetLevel = WorldTiles.ElementWithKey(mapLevel);
            //Target doesn't exist
            if (targetLevel == null)
                return;
            //Find the tile
            //Get the position to affect
            int xInteger = (int)Math.Floor(x);
            int yInteger = (int)Math.Floor(y);
            //Add the entity
            WorldTile worldTile = targetLevel.Get(xInteger, yInteger);
            if (worldTile == null)
                return;
            worldTile.Remove(entity);
        }

    }
}
