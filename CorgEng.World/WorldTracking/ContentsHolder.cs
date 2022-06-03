using CorgEng.EntityComponentSystem.Entities;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.World;
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

        private IEntity[] contentsArray = new IEntity[DEFAULT_CONTENT_ARRAY_SIZE];

        /// <summary>
        /// Maximum value used in the array
        /// </summary>
        private int nextInsertionPointer = 0;

        public IEnumerable<IEntity> GetContents()
        {
            throw new NotImplementedException();
        }

        public void Insert(IEntity entity)
        {
            //Array needs growing
            while (nextInsertionPointer == contentsArray.Length)
            {
                IEntity[] arrayRef = contentsArray;
                contentsArray = new Entity[arrayRef.Length * ARRAY_GROWTH_FACTORY];
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
            entity.ContentsIndex = nextInsertionPointer;
            contentsArray[nextInsertionPointer] = entity;
            //Insert the next entity in the next position
            nextInsertionPointer++;
        }

        public void Remove(IEntity entity)
        {
            //Validation check
            if (contentsArray[entity.ContentsIndex] != entity)
                throw new Exception($"Invalid entity in WorldTile array, entity claims to be at position {entity.ContentsIndex}");
            //Remove the entity
            contentsArray[entity.ContentsIndex] = null;
            entity.ContentsIndex = -1;
            //Increase the fragmentation factor
            fragmentationFactor++;
        }

    }
}
