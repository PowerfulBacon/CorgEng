using CorgEng.UtilityTypes.Batches;
using CorgEng.UtilityTypes.Batches.Interfaces;
using CorgEng.UtilityTypes.BindableProperties;
using CorgEng.UtilityTypes.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.UtilityTypes
{
    [TestClass]
    public class BatchTests
    {

        private class TestBatch : Batch<TestBatch>
        {
            public TestBatch(int batchSize = 25000) : base(batchSize)
            { }

            public override int[] BatchVectorSizes => new int[] { 3, 2, 1, 3 };
        }

        [TestMethod]
        public void TestBatches()
        {
            TestBatch batch = new TestBatch(5);
            Assert.AreEqual(0, batch.Count);
            Assert.AreEqual(0, batch.IndividualBatchCounts);
            BatchElement<TestBatch> first = new BatchElement<TestBatch>(new BindableProperty<Vector<float>>[] {
                new BindableProperty<Vector<float>>(new Vector<float>(1, 2, 3)),
                new BindableProperty<Vector<float>>(new Vector<float>(4, 5)),
                new BindableProperty<Vector<float>>(new Vector<float>(6)),
                new BindableProperty<Vector<float>>(new Vector<float>(7, 8, 9)),
            });
            batch.Add(first);
            Console.WriteLine(batch);
            //Check size
            Assert.AreEqual(1, batch.Count);
            Assert.AreEqual(1, batch.IndividualBatchCounts);
            //Multi-add
            for (int i = 1; i < 5; i ++)
            {
                batch.Add(new BatchElement<TestBatch>(new BindableProperty<Vector<float>>[] {
                    new BindableProperty<Vector<float>>(new Vector<float>(i, i, i)),
                    new BindableProperty<Vector<float>>(new Vector<float>(i, i)),
                    new BindableProperty<Vector<float>>(new Vector<float>(i)),
                    new BindableProperty<Vector<float>>(new Vector<float>(i, i, i)),
                }));
                Assert.AreEqual(i + 1, batch.Count);
                Assert.AreEqual(1, batch.IndividualBatchCounts);
            }
            //Add another batch to test expanding
            BatchElement<TestBatch> specialElement = new BatchElement<TestBatch>(new BindableProperty<Vector<float>>[] {
                new BindableProperty<Vector<float>>(new Vector<float>(1, 2, 3)),
                new BindableProperty<Vector<float>>(new Vector<float>(4, 5)),
                new BindableProperty<Vector<float>>(new Vector<float>(9)),
                new BindableProperty<Vector<float>>(new Vector<float>(7, 8, 9)),
            });
            batch.Add(specialElement);
            Console.WriteLine(batch);
            //Add a new batch
            Assert.AreEqual(6, batch.Count);
            Assert.AreEqual(2, batch.IndividualBatchCounts);
            //Access specific elements
            //Element 0:
            float[] firstArray = batch.GetArray(0, 0);
            Assert.AreEqual(1, firstArray[0]);
            Assert.AreEqual(2, firstArray[1]);
            Assert.AreEqual(3, firstArray[2]);
            float[] firstArrayElementTwo = batch.GetArray(0, 1);
            Assert.AreEqual(4, firstArrayElementTwo[0]);
            Assert.AreEqual(5, firstArrayElementTwo[1]);
            //Element 5:
            float[] secondArray = batch.GetArray(1, 2);
            Assert.AreEqual(9, secondArray[0]);
            Assert.AreEqual(default, secondArray[1]);
            //Test removal
            batch.Remove(specialElement);
            Console.WriteLine(batch);
            Assert.AreEqual(5, batch.Count);
            Assert.AreEqual(1, batch.IndividualBatchCounts);
            //Add the last element back (so we know the values)
            batch.Add(specialElement);
            //Remove the first element
            batch.Remove(first);
            Assert.AreEqual(5, batch.Count);
            Assert.AreEqual(1, batch.IndividualBatchCounts);
            //Check the first element
            Console.WriteLine(batch);
            Assert.AreEqual(1, batch.GetArray(0, 0)[0]);
            Assert.AreEqual(2, batch.GetArray(0, 0)[1]);
            Assert.AreEqual(3, batch.GetArray(0, 0)[2]);
            Assert.AreEqual(4, batch.GetArray(0, 1)[0]);
            Assert.AreEqual(5, batch.GetArray(0, 1)[1]);
            Assert.AreEqual(9, batch.GetArray(0, 2)[0]);
            Assert.AreEqual(7, batch.GetArray(0, 3)[0]);
            Assert.AreEqual(8, batch.GetArray(0, 3)[1]);
            Assert.AreEqual(9, batch.GetArray(0, 3)[2]);
        }

    }
}
