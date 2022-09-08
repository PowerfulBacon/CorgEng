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
        internal int nextInsertionPointer = 0;

        private Vector<int> position;

        public ContentsHolder(int x, int y)
        {
            position = new Vector<int>(x, y);
        }

        public IEnumerable<IWorldTrackComponent> GetContents()
        {
            return new ContentsEnumerable(this);
        }

        public void Insert(IWorldTrackComponent entity)
        {
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
                        contentsArray[startPointer++] = contentsArray[endPointer];
                    }
                    endPointer++;
                }
                //Defragmentation is completed
                nextInsertionPointer = startPointer;
                fragmentationFactor = 0;
            }
            //Insert the entity
            entity.ContentsIndexPosition = nextInsertionPointer;
            entity.ContentsLocation = position;
            contentsArray[nextInsertionPointer] = entity;
            //Insert the next entity in the next position
            nextInsertionPointer++;
        }

        public void Remove(IWorldTrackComponent entity)
        {
            //Validation check
            if (entity.ContentsIndexPosition >= contentsArray.Length || entity.ContentsIndexPosition < 0 || contentsArray[entity.ContentsIndexPosition] != entity)
                throw new Exception($"Invalid entity in WorldTile array, entity claims to be at position {entity.ContentsIndexPosition}");
            //Remove the entity
            contentsArray[entity.ContentsIndexPosition] = null;
            entity.ContentsIndexPosition = -1;
            //Increase the fragmentation factor
            fragmentationFactor++;
        }

    }
}
