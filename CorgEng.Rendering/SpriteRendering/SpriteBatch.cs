using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    internal sealed class SpriteBatch : IBatch<SpriteBatch>
    {

        public int[] BatchVectorSizes => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public int BatchSize => throw new NotImplementedException();

        public int IndividualBatchCounts => throw new NotImplementedException();

        public void Add(IBatchElement<SpriteBatch> element)
        {
            throw new NotImplementedException();
        }

        public float[] GetArray(int batchIndex, int groupIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IBatchElement<SpriteBatch>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Remove(IBatchElement<SpriteBatch> element)
        {
            throw new NotImplementedException();
        }

        public void Update(int batchIndex, int groupIndex, IVector<float> newValue)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
