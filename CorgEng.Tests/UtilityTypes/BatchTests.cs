using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Batches;
using CorgEng.UtilityTypes.BindableProperties;
using CorgEng.UtilityTypes.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CorgEng.Tests.UtilityTypes
{
    [TestClass]
    public class BatchTests : TestBase
    {

        private Vector<float> TestVector { get; } = new Vector<float>();

        private class TestBatch : Batch<TestBatch>
        {
            public TestBatch(int batchSize = 25000) : base(batchSize)
            { }

            public override int[] BatchVectorSizes => new int[] { 3, 2, 1, 3 };
        }

        [TestMethod]
        public void TestBatchMassAdd()
        {
            TestBatch batch = new TestBatch();
            BatchElement<TestBatch> first = new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(1, 1, 1)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9, 9)),
            });
            batch.Add(first);
            BatchElement<TestBatch> second = new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(2, 2, 2)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9, 9)),
            });
            batch.Add(second);
            BatchElement<TestBatch> third = new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(3, 3, 3)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9, 9)),
            });
            batch.Add(third);
            BatchElement<TestBatch> forth = new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(4, 4, 4)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9, 9)),
            });
            batch.Add(forth);
            Assert.AreEqual(1, batch.GetArray(0, 0)[0]);
            Assert.AreEqual(1, batch.GetArray(0, 0)[1]);
            Assert.AreEqual(1, batch.GetArray(0, 0)[2]);
            Assert.AreEqual(2, batch.GetArray(0, 0)[3]);
            Assert.AreEqual(2, batch.GetArray(0, 0)[4]);
            Assert.AreEqual(2, batch.GetArray(0, 0)[5]);
            Assert.AreEqual(3, batch.GetArray(0, 0)[6]);
            Assert.AreEqual(3, batch.GetArray(0, 0)[7]);
            Assert.AreEqual(3, batch.GetArray(0, 0)[8]);
            Assert.AreEqual(4, batch.GetArray(0, 0)[9]);
            Assert.AreEqual(4, batch.GetArray(0, 0)[10]);
            Assert.AreEqual(4, batch.GetArray(0, 0)[11]);
        }

        [TestMethod]
        public void TestBatchUpdate()
        {
            TestBatch batch = new TestBatch();
            BatchElement<TestBatch> first = new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(1, 1, 1)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9, 9)),
            });
            batch.Add(first);
            BatchElement<TestBatch> second = new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(2, 2, 2)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9, 9)),
            });
            batch.Add(second);
            BindableProperty<IVector<float>>[] bp = new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(3, 3, 3)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9, 9)),
            };
            BatchElement<TestBatch> third = new BatchElement<TestBatch>(bp);
            batch.Add(third);
            BatchElement<TestBatch> forth = new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(4, 4, 4)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9)),
                new BindableProperty<IVector<float>>(new Vector<float>(9, 9, 9)),
            });
            batch.Add(forth);
            //Change BP
            bp[0].Value = new Vector<float>(6, 6, 6);
            Assert.AreEqual(6, batch.GetArray(0, 0)[6], $"{batch}");
            Assert.AreEqual(6, batch.GetArray(0, 0)[7], $"{batch}");
            Assert.AreEqual(6, batch.GetArray(0, 0)[8], $"{batch}");
        }

        [TestMethod]
        public void TestBatches()
        {
            TestBatch batch = new TestBatch(5);
            Assert.AreEqual(0, batch.Count);
            Assert.AreEqual(0, batch.IndividualBatchCounts);
            BatchElement<TestBatch> first = new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(1, 2, 3)),
                new BindableProperty<IVector<float>>(new Vector<float>(4, 5)),
                new BindableProperty<IVector<float>>(new Vector<float>(6)),
                new BindableProperty<IVector<float>>(new Vector<float>(7, 8, 9)),
            });
            batch.Add(first);
            Console.WriteLine(batch);
            //Check size
            Assert.AreEqual(1, batch.Count);
            Assert.AreEqual(1, batch.IndividualBatchCounts);
            //Multi-add
            for (int i = 1; i < 5; i ++)
            {
                batch.Add(new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                    new BindableProperty<IVector<float>>(new Vector<float>(i, i, i)),
                    new BindableProperty<IVector<float>>(new Vector<float>(i, i)),
                    new BindableProperty<IVector<float>>(new Vector<float>(i)),
                    new BindableProperty<IVector<float>>(new Vector<float>(i, i, i)),
                }));
                Assert.AreEqual(i + 1, batch.Count);
                Assert.AreEqual(1, batch.IndividualBatchCounts);
            }
            //Add another batch to test expanding
            BatchElement<TestBatch> specialElement = new BatchElement<TestBatch>(new BindableProperty<IVector<float>>[] {
                new BindableProperty<IVector<float>>(new Vector<float>(1, 2, 3)),
                new BindableProperty<IVector<float>>(new Vector<float>(4, 5)),
                new BindableProperty<IVector<float>>(new Vector<float>(9)),
                new BindableProperty<IVector<float>>(new Vector<float>(7, 8, 9)),
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
