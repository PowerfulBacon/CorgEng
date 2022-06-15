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

        private IBinaryList<IPositionBasedBinaryList<ContentsHolder>> _worldtiles;

        private IBinaryList<IPositionBasedBinaryList<ContentsHolder>> WorldTiles
        {
            get
            {
                if (_worldtiles == null)
                    _worldtiles = BinaryListFactory.CreateEmpty<IPositionBasedBinaryList<ContentsHolder>>();
                return _worldtiles;
            }
        }

        public void AddEntity(IEntity entity, double x, double y, int mapLevel)
        {
            IPositionBasedBinaryList<ContentsHolder> targetLevel = WorldTiles.ElementWithKey(mapLevel);
            //Get the z-level to affect
            if (targetLevel == null)
            {
                targetLevel = PositionBasedBinaryListFactory.CreateEmpty<ContentsHolder>();
                WorldTiles.Add(mapLevel, targetLevel);
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

        public IContentsHolder GetContentsAt(double x, double y, int mapLevel)
        {
            return WorldTiles.ElementWithKey(mapLevel)?.Get((int)Math.Floor(x), (int)Math.Floor(y));
        }

        public void RemoveEntity(IEntity entity, double x, double y, int mapLevel)
        {
            IPositionBasedBinaryList<ContentsHolder> targetLevel = WorldTiles.ElementWithKey(mapLevel);
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

    }
}
