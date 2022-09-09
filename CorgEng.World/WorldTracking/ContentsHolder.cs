#define DEBUG_MODE

using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.World;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.World.WorldTracking
{
    internal class ContentsHolder : IContentsHolder
    {

        /// <summary>
        /// The proportion of the array that is fragmented, before it is rebuilt
        /// </summary>
        private const float FRAGMENTATION_PROPORTION_LIMIT = 0.3f;

        /// <summary>
        /// The amount the array will multiply by with each expansion
        /// </summary>
        private const int ARRAY_GROWTH_FACTORY = 2;

        /// <summary>
        /// The default size of the contents array
        /// </summary>
        private const int DEFAULT_CONTENT_ARRAY_SIZE = 4;

        /// <summary>
        /// The amount of empty space within the array
        /// </summary>
        private int fragmentationFactor = 0;

        internal IWorldTrackComponent[] contentsArray = new IWorldTrackComponent[DEFAULT_CONTENT_ARRAY_SIZE];

        /// <summary>
        /// Maximum value used in the array
        /// </summary>
        internal volatile int nextInsertionPointer = 0;

        private Vector<int> position;

        public int Count { get; set; } = 0;

        public ContentsHolder(int x, int y)
        {
            position = new Vector<int>(x, y);
        }

        public IEnumerable<IWorldTrackComponent> GetContents()
        {
            return new ContentsEnumerable(this);
        }

        private object _lock = new object();

        public void Insert(IWorldTrackComponent entity)
        {
            lock (_lock)
            {
                if (entity.ContentsIndexPosition != -1)
                {
                    throw new Exception("Attempting to isnert an entity into a contents holder while it already is inside anotehr contents holder.");
                }
                //Array needs growing
                while (nextInsertionPointer == contentsArray.Length)
                {
                    IWorldTrackComponent[] arrayRef = contentsArray;
                    contentsArray = new IWorldTrackComponent[arrayRef.Length * ARRAY_GROWTH_FACTORY];
                    arrayRef.CopyTo(contentsArray, 0);
                }
                //Array needs defragmenting
                if (fragmentationFactor >= (nextInsertionPointer + 1) * FRAGMENTATION_PROPORTION_LIMIT)
                {
                    int startPointer = 0;
                    int endPointer = 0;
                    while (endPointer < nextInsertionPointer)
                    {
                        if (contentsArray[endPointer] != null)
                        {
                            //Shift the element
                            contentsArray[startPointer] = contentsArray[endPointer];
                            contentsArray[startPointer].ContentsIndexPosition = startPointer;
                            if (endPointer != startPointer)
                                contentsArray[endPointer] = null;
                            startPointer++;
                        }
                        endPointer++;
                    }
                    //Defragmentation is completed
                    nextInsertionPointer = startPointer;
                    fragmentationFactor = 0;
                }
                //Increase count
                Count++;
                //Insert the entity
                entity.ContentsIndexPosition = nextInsertionPointer;
                entity.ContentsLocation = position;
#if DEBUG_MODE
                if (contentsArray[nextInsertionPointer] != null)
                    throw new Exception("Something already exists at the insertion pointer.");
#endif
                contentsArray[nextInsertionPointer] = entity;
                //Insert the next entity in the next position
                nextInsertionPointer++;
            }
        }

        public void Remove(IWorldTrackComponent entity)
        {
            lock (_lock)
            {
                //Validation check
                if (entity.ContentsIndexPosition >= contentsArray.Length || entity.ContentsIndexPosition < 0 || contentsArray[entity.ContentsIndexPosition] != entity)
                {
                    int locatedIndex = -1;
                    int i = 0;
                    foreach (IWorldTrackComponent element in contentsArray)
                    {
                        if (element == entity)
                        {
                            locatedIndex = i;
                            break;
                        }
                        i++;
                    }
                    throw new Exception($"Invalid entity in WorldTile array, entity claims to be at position {entity.ContentsIndexPosition}. It was actually found at {locatedIndex}!");
                }
                //Remove the entity
                contentsArray[entity.ContentsIndexPosition] = null;
                entity.ContentsIndexPosition = -1;
                //Decrease count
                Count--;
                //Increase the fragmentation factor
                fragmentationFactor++;
            }
        }

    }
}
