using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;

namespace CorgEng.UtilityTypes.Batches.Interfaces
{
    public interface IBatch<Self> : IEnumerable<IBatchElement<Self>>
        where Self : IBatch<Self>
    {

        //The lengths of the groupings
        //A value of 3 will concatenate 3 values into a single vec3 (x, y, z)
        int[] BatchVectorSizes { get; }

        //Add an element to the render batch
        void Add(IBatchElement<Self> element);

        //Remove an element from the rendering batch
        void Remove(IBatchElement<Self> element);

        //Update some element in the batch
        void Update(int batchIndex, int groupIndex, Vector<float> newValue);

        float[] GetArray(int batchIndex, int groupIndex);

    }
}
