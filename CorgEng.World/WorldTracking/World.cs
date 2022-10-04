using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.BinaryLists;
using CorgEng.GenericInterfaces.World;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Concurrent;
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

        private IDictionary<string, IBinaryList<IPositionBasedBinaryList<IContentsHolder>>> _worldtiles;

        private IDictionary<string, IBinaryList<IPositionBasedBinaryList<IContentsHolder>>> WorldTiles
        {
            get
            {
                if (_worldtiles == null)
                    _worldtiles = new Dictionary<string, IBinaryList<IPositionBasedBinaryList<IContentsHolder>>>();
                return _worldtiles;
            }
        }

        public IVector<int> GetGridPosition(IVector<float> sourcePosition)
        {
            return new Vector<int>(
                (int)Math.Floor(sourcePosition.X),
                (int)Math.Floor(sourcePosition.Y)
                );
        }

        public void AddEntity(string trackKey, IWorldTrackComponent trackComponent, double x, double y, int mapLevel)
        {
            //World is pretty important, so single thread it
            lock (this)
            {
                if (trackComponent.ContentsIndexPosition != -1)
                    throw new Exception($"Attempting to insert an entity while it is already in another location");
                if (!WorldTiles.ContainsKey(trackKey))
                {
                    WorldTiles.TryAdd(trackKey, BinaryListFactory.CreateEmpty<IPositionBasedBinaryList<IContentsHolder>>());
                }
                IPositionBasedBinaryList<IContentsHolder> targetLevel = WorldTiles[trackKey].ElementWithKey(mapLevel);
                //Get the z-level to affect
                if (targetLevel == null)
                {
                    targetLevel = PositionBasedBinaryListFactory.CreateEmpty<IContentsHolder>();
                    WorldTiles[trackKey].Add(mapLevel, targetLevel);
                }
                //Get the position to affect
                IVector<int> tilePosition = GetGridPosition(new Vector<float>((float)x, (float)y));
                int xInteger = tilePosition.X;
                int yInteger = tilePosition.Y;
                //Add the entity
                IContentsHolder worldTile = targetLevel.Get(xInteger, yInteger);
                if (worldTile == null)
                {
                    worldTile = new ContentsHolder(xInteger, yInteger);
                    targetLevel.Add(xInteger, yInteger, worldTile);
                }
                worldTile.Insert(trackComponent);
            }
        }

        public void AddEntity(IWorldTrackComponent trackComponent, double x, double y, int mapLevel)
        {
            AddEntity("_world", trackComponent, x, y, mapLevel);
        }

        public IPositionBasedBinaryList<IContentsHolder> GetContents(int mapLevel)
        {
            return GetContents("_world", mapLevel);
        }

        public IPositionBasedBinaryList<IContentsHolder> GetContents(string trackKey, int mapLevel)
        {
            lock (this)
            {
                if (!WorldTiles.ContainsKey(trackKey))
                    return null;
                return WorldTiles[trackKey].ElementWithKey(mapLevel);
            }
        }

        public IContentsHolder GetContentsAt(string trackKey, double x, double y, int mapLevel)
        {
            lock (this)
            {
                if (!WorldTiles.ContainsKey(trackKey))
                    return null;
                IVector<int> gridPosition = GetGridPosition(new Vector<float>((float)x, (float)y));
                return WorldTiles[trackKey].ElementWithKey(mapLevel)?.Get(gridPosition.X, gridPosition.Y);
            }
        }

        public IContentsHolder GetContentsAt(double x, double y, int mapLevel)
        {
            return GetContentsAt("_world", x, y, mapLevel);
        }

        public void RemoveEntity(string trackKey, IWorldTrackComponent trackComponent, double x, double y, int mapLevel)
        {
            lock (this)
            {
                if (trackComponent.ContentsIndexPosition == -1)
                    throw new Exception($"Attempting to remove an entity from an invalid location ({x}, {y})");
                IPositionBasedBinaryList<IContentsHolder> targetLevel = WorldTiles[trackKey].ElementWithKey(mapLevel);
                //Target doesn't exist
                if (targetLevel == null)
                    return;
                //Find the tile
                //Get the position to affect
                IVector<int> tilePosition = GetGridPosition(new Vector<float>((float)x, (float)y));
                int xInteger = tilePosition.X;
                int yInteger = tilePosition.Y;
                //Add the entity
                IContentsHolder worldTile = targetLevel.Get(xInteger, yInteger);
                if (worldTile == null)
                    return;
                worldTile.Remove(trackComponent);
                //If the world tile is empty, remove it from the world
                if (worldTile.Count == 0)
                {
                    targetLevel.Remove(xInteger, yInteger);
                }
            }
        }

        public void RemoveEntity(IWorldTrackComponent trackComponent, double x, double y, int mapLevel)
        {
            RemoveEntity("_world", trackComponent, x, y, mapLevel);
        }
    }
}
