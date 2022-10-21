using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CorgEng.UtilityTypes.Batches
{
    public abstract class Batch<T> : IBatch<T>
        where T : Batch<T>
    {

        public const int DEFAULT_BATCH_SIZE = 25000;

        //Groupings of elements within this batch
        public abstract int[] BatchVectorSizes { get; }

        //Amount of elements in this batch
        //A vec3 would be accessed by
        //batchElements[0][0]   //x
        //batchElements[0][1]   //y
        //batchElements[0][2]   //z
        private List<float[][]> batchElements;

        private List<IBatchElement<T>[]> batchElementRefs;

        //Count of elements in the batch
        //Represents:
        // - How many items are held by this batch (Counts start from 1)
        // - The index of where to insert next (Indexing starts from 0)
        public int Count { get; private set; } = 0;

        //The size of the batches
        public int BatchSize { get; }

        //The amount of individual batches
        public int IndividualBatchCounts => batchElements.Count;

        public Batch(int batchSize = DEFAULT_BATCH_SIZE)
        {
            //Set the size of the batches
            //Once this size is exceeded, a new array will be created to hold the new values
            BatchSize = batchSize;
            //Set the default elements of the batch
            batchElements = new List<float[][]>();
            batchElementRefs = new List<IBatchElement<T>[]>();
        }

        /// <summary>
        /// Expand this batch by BatchSize
        /// </summary>
        private void ExpandBatch()
        {
            lock (this)
            {
                float[][] newBatchGroup = new float[BatchVectorSizes.Length][];
                IBatchElement<T>[] batchElementRefGroup = new IBatchElement<T>[BatchSize];
                for (int i = 0; i < BatchVectorSizes.Length; i++)
                {
                    //Set each batch group size accordingly
                    //A float needs 25000 space
                    //A vec3 needs 75000 space
                    newBatchGroup[i] = new float[BatchSize * BatchVectorSizes[i]];
                }
                batchElements.Add(newBatchGroup);
                batchElementRefs.Add(batchElementRefGroup);
            }
        }

        /// <summary>
        /// Adds an element to the batch
        /// </summary>
        /// <param name="element"></param>
        public void Add(IBatchElement<T> element)
        {
            lock (this)
            {
                //Check if the batch needs expanding
                if (Count % BatchSize == 0)
                {
                    //The batch size needs expanding
                    ExpandBatch();
                }
                //Locate our batch group index
                int batchGroupIndex = Count / BatchSize;
                int internalGroupIndex = Count % BatchSize;
                element.ContainingBatch = this;
                element.BatchPosition = Count;
                //Store the reference
                batchElementRefs[batchGroupIndex][internalGroupIndex] = element;
                //Perform batch insertion
                for (int i = 0; i < BatchVectorSizes.Length; i++)
                {
                    for (int j = 0; j < BatchVectorSizes[i]; j++)
                    {
                        batchElements[batchGroupIndex][i][internalGroupIndex * BatchVectorSizes[i] + j] = element.GetValue(i)[j];
                    }
                }
                //Increment the count
                Count++;
            }
        }

        /// <summary>
        /// Removes an element from the batch.
        /// Swaps the last element in the batch out for this element
        /// </summary>
        /// <param name="element"></param>
        public void Remove(IBatchElement<T> element)
        {
            lock (this)
            {
                int currentBatchGroup = element.BatchPosition / BatchSize;
                int lastBatchGroup = (Count - 1) / BatchSize;
                //Replace element with the last element
                //If we are removing the last element, go straight to the easy delete
                if (Count - 1 != element.BatchPosition)
                {
                    for (int groupIndex = 0; groupIndex < BatchVectorSizes.Length; groupIndex++)
                    {
                        for (int i = 0; i < BatchVectorSizes[groupIndex]; i++)
                        {
                            //Put the last element in the currents position
                            batchElements[currentBatchGroup][groupIndex][element.BatchPosition % BatchSize * BatchVectorSizes[groupIndex] + i]
                                = batchElements[lastBatchGroup][groupIndex][(Count - 1) % BatchSize * BatchVectorSizes[groupIndex] + i];
                        }
                    }
                }
                //Update the reference
                batchElementRefs[currentBatchGroup][element.BatchPosition % BatchSize]
                    = batchElementRefs[lastBatchGroup][(Count - 1) % BatchSize];
                batchElementRefs[lastBatchGroup][(Count - 1) % BatchSize].BatchPosition = currentBatchGroup * BatchSize + (element.BatchPosition % BatchSize);
                batchElementRefs[lastBatchGroup][(Count - 1) % BatchSize] = null;
                //Decrement the count
                Count--;
                //Remove the last batch if it is now empty
                if (Count - 1 % BatchSize == BatchSize - 1)
                {
                    batchElements.RemoveAt(batchElements.Count - 1);
                }
            }
        }

        /// <summary>
        /// Updates a specific element in the batch
        /// </summary>
        /// <param name="batchIndex"></param>
        /// <param name="groupIndex"></param>
        /// <param name="newValue"></param>
        public void Update(int batchIndex, int groupIndex, IVector<float> newValue)
        {
            lock (this)
            {
                //Find what array group we are in
                int targetArrayGroupIndex = GetArrayGroupIndex(batchIndex, groupIndex);
                int targetArrayIndividualIndex = (BatchVectorSizes[groupIndex] * batchIndex) % (BatchVectorSizes[groupIndex] * BatchSize);
                for (int i = 0; i < BatchVectorSizes[groupIndex]; i++)
                {
                    batchElements[targetArrayGroupIndex][groupIndex][targetArrayIndividualIndex + i] = newValue[i];
                }
            }
        }

        /// <summary>
        /// Converts an int into a batch group
        /// For a batch storing floats of size 25000:
        /// 0 = 0
        /// 1000 = 0
        /// 24999 = 0
        /// 25000 = 1
        /// </summary>
        private int GetArrayGroupIndex(int batchIndex, int groupIndex)
        {
            return batchIndex / (BatchVectorSizes[groupIndex] * BatchSize);
        }

        public IEnumerator<IBatchElement<T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public float[] GetArray(int batchIndex, int groupIndex)
        {
            lock (this)
            {
                return batchElements[batchIndex][groupIndex];
            }
        }

        public override string ToString()
        {
            string[] batchElementGroups = new string[batchElements.Count];
            for (int i = 0; i < batchElements.Count; i++)
            {
                string[] batchElementGroupContents = new string[batchElements[i].Length];
                for (int j = 0; j < batchElements[i].Length; j++)
                {
                    string[] values = new string[batchElements[i][j].Length];
                    for (int k = 0; k < batchElements[i][j].Length; k++)
                    {
                        values[k] = batchElements[i][j][k].ToString();
                    }
                    batchElementGroupContents[j] = $"{{{string.Join(",", values)}}}";
                }
                batchElementGroups[i] = $"{string.Join(",\n\t", batchElementGroupContents)}";
            }
            return $"Batch (Count: {Count}): \n{{\n\t{string.Join(",\n\t", batchElementGroups)}\n}}";
        }
    }
}
