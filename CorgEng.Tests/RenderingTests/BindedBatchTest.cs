using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.UtilityTypes.Batches;
using CorgEng.UtilityTypes.BindableProperties;
using CorgEng.UtilityTypes.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.RenderingTests
{
    [TestClass]
    public class BindedBatchTest
    {

        private class TestBatch : Batch<TestBatch>
        {
            public TestBatch(int batchSize = 100) : base(batchSize)
            { }

            public override int[] BatchVectorSizes => new int[] { 3, 2, 1 };
        }

        [TestMethod]
        public void TestBatches()
        {
            //Create first property and add it
            IBindableProperty<IVector<float>> property1 = new BindableProperty<IVector<float>>(new Vector<float>(1, 1, 1));
            IBindableProperty<IVector<float>> property2 = new BindableProperty<IVector<float>>(new Vector<float>(1, 1));
            IBindableProperty<IVector<float>> property3 = new BindableProperty<IVector<float>>(new Vector<float>(1));

            TestBatch batch = new TestBatch();

            IBatchElement<TestBatch> batchElement = new BatchElement<TestBatch>(
                new IBindableProperty<IVector<float>>[] {
                    property1,
                    property2,
                    property3,
                });
            batch.Add(batchElement);

            //Test adding

            Assert.AreEqual(0, batchElement.BatchPosition, "Expected 1st element in place 0.");
            Assert.AreEqual(1, batch.GetArray(0, 0)[batchElement.BatchPosition + 0], "Batch adding failure");
            Assert.AreEqual(1, batch.GetArray(0, 0)[batchElement.BatchPosition + 1], "Batch adding failure");
            Assert.AreEqual(1, batch.GetArray(0, 0)[batchElement.BatchPosition + 2], "Batch adding failure");
            Assert.AreEqual(1, batch.GetArray(0, 1)[batchElement.BatchPosition + 0], "Batch adding failure");
            Assert.AreEqual(1, batch.GetArray(0, 1)[batchElement.BatchPosition + 1], "Batch adding failure");
            Assert.AreEqual(1, batch.GetArray(0, 2)[batchElement.BatchPosition + 0], "Batch adding failure");

            //Test updating

            property1.Value.X = 2;
            property1.Value.Y = 2;
            property1.Value.Z = 2;
            property1.TriggerChanged();
            property2.Value.X = 2;
            property2.Value.Y = 2;
            property2.TriggerChanged();
            property3.Value.X = 2;
            property3.TriggerChanged();

            Assert.AreEqual(2, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 0], "Batch updating failure");
            Assert.AreEqual(2, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 1], "Batch updating failure");
            Assert.AreEqual(2, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 2], "Batch updating failure");
            Assert.AreEqual(2, batch.GetArray(0, 1)[2 * batchElement.BatchPosition + 0], "Batch updating failure");
            Assert.AreEqual(2, batch.GetArray(0, 1)[2 * batchElement.BatchPosition + 1], "Batch updating failure");
            Assert.AreEqual(2, batch.GetArray(0, 2)[1 * batchElement.BatchPosition + 0], "Batch updating failure");

            //Add another element

            IBindableProperty<IVector<float>> property12 = new BindableProperty<IVector<float>>(new Vector<float>(3, 3, 3));
            IBindableProperty<IVector<float>> property22 = new BindableProperty<IVector<float>>(new Vector<float>(3, 3));
            IBindableProperty<IVector<float>> property32 = new BindableProperty<IVector<float>>(new Vector<float>(3));
            IBatchElement<TestBatch> batchElement2 = new BatchElement<TestBatch>(
                new IBindableProperty<IVector<float>>[] {
                    property12,
                    property22,
                    property32,
                });
            batch.Add(batchElement2);

            //Test adding

            Assert.AreEqual(1, batchElement2.BatchPosition, "Expected 2nd element in place 1.");
            Assert.AreEqual(3, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 0], "Batch second add failure");
            Assert.AreEqual(3, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 1], "Batch second add failure");
            Assert.AreEqual(3, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 2], "Batch second add failure");
            Assert.AreEqual(3, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 0], "Batch second add failure");
            Assert.AreEqual(3, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 1], "Batch second add failure");
            Assert.AreEqual(3, batch.GetArray(0, 2)[1 * batchElement2.BatchPosition + 0], "Batch second add failure");

            //Test updating

            property12.Value.X = 4;
            property12.Value.Y = 4;
            property12.Value.Z = 4;
            property12.TriggerChanged();
            property22.Value.X = 4;
            property22.Value.Y = 4;
            property22.TriggerChanged();
            property32.Value.X = 4;
            property32.TriggerChanged();

            Assert.AreEqual(4, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 0], "Batch second update failure");
            Assert.AreEqual(4, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 1], "Batch second update failure");
            Assert.AreEqual(4, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 2], "Batch second update failure");
            Assert.AreEqual(4, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 0], "Batch second update failure");
            Assert.AreEqual(4, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 1], "Batch second update failure");
            Assert.AreEqual(4, batch.GetArray(0, 2)[1 * batchElement2.BatchPosition + 0], "Batch second update failure");

            //Remove
            batch.Remove(batchElement);

            //Test
            Assert.AreEqual(4, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 0], "Batch removal failure");
            Assert.AreEqual(4, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 1], "Batch removal failure");
            Assert.AreEqual(4, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 2], "Batch removal failure");
            Assert.AreEqual(4, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 0], "Batch removal failure");
            Assert.AreEqual(4, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 1], "Batch removal failure");
            Assert.AreEqual(4, batch.GetArray(0, 2)[1 * batchElement2.BatchPosition + 0], "Batch removal failure");

            //Test changing

            property12.Value.X = 5;
            property12.Value.Y = 5;
            property12.Value.Z = 5;
            property12.TriggerChanged();
            property22.Value.X = 5;
            property22.Value.Y = 5;
            property22.TriggerChanged();
            property32.Value.X = 5;
            property32.TriggerChanged();

            Assert.AreEqual(5, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 0], "Batch update post-removal failure");
            Assert.AreEqual(5, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 1], "Batch update post-removal failure");
            Assert.AreEqual(5, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 2], "Batch update post-removal failure");
            Assert.AreEqual(5, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 0], "Batch update post-removal failure");
            Assert.AreEqual(5, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 1], "Batch update post-removal failure");
            Assert.AreEqual(5, batch.GetArray(0, 2)[1 * batchElement2.BatchPosition + 0], "Batch update post-removal failure");

            batch.Add(batchElement);

            //Test adding

            Assert.AreEqual(2, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 0], "Adding A, Adding B, Removing A, Adding A results in an incorrect batch configuration.");
            Assert.AreEqual(2, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 1], "Adding A, Adding B, Removing A, Adding A results in an incorrect batch configuration.");
            Assert.AreEqual(2, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 2], "Adding A, Adding B, Removing A, Adding A results in an incorrect batch configuration.");
            Assert.AreEqual(2, batch.GetArray(0, 1)[2 * batchElement.BatchPosition + 0], "Adding A, Adding B, Removing A, Adding A results in an incorrect batch configuration.");
            Assert.AreEqual(2, batch.GetArray(0, 1)[2 * batchElement.BatchPosition + 1], "Adding A, Adding B, Removing A, Adding A results in an incorrect batch configuration.");
            Assert.AreEqual(2, batch.GetArray(0, 2)[1 * batchElement.BatchPosition + 0], "Adding A, Adding B, Removing A, Adding A results in an incorrect batch configuration.");

            //Test updating

            property1.Value.X = 6;
            property1.Value.Y = 6;
            property1.Value.Z = 6;
            property1.TriggerChanged();
            property2.Value.X = 6;
            property2.Value.Y = 6;
            property2.TriggerChanged();
            property3.Value.X = 6;
            property3.TriggerChanged();

            Assert.AreEqual(6, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 0], "Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 1], "Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 2], "Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 1)[2 * batchElement.BatchPosition + 0], "Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 1)[2 * batchElement.BatchPosition + 1], "Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 2)[1 * batchElement.BatchPosition + 0], "Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(5, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 0], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(5, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 1], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(5, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 2], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(5, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 0], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(5, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 1], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");
            Assert.AreEqual(5, batch.GetArray(0, 2)[1 * batchElement2.BatchPosition + 0], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating A results in an incorrect batch configuration");

            //Test changing

            property12.Value.X = 7;
            property12.Value.Y = 7;
            property12.Value.Z = 7;
            property12.TriggerChanged();
            property22.Value.X = 7;
            property22.Value.Y = 7;
            property22.TriggerChanged();
            property32.Value.X = 7;
            property32.TriggerChanged();

            Assert.AreEqual(7, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 0], "Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(7, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 1], "Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(7, batch.GetArray(0, 0)[3 * batchElement2.BatchPosition + 2], "Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(7, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 0], "Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(7, batch.GetArray(0, 1)[2 * batchElement2.BatchPosition + 1], "Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(7, batch.GetArray(0, 2)[1 * batchElement2.BatchPosition + 0], "Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 0], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 1], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 0)[3 * batchElement.BatchPosition + 2], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 1)[2 * batchElement.BatchPosition + 0], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 1)[2 * batchElement.BatchPosition + 1], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");
            Assert.AreEqual(6, batch.GetArray(0, 2)[1 * batchElement.BatchPosition + 0], "(UNEXPECTED SIDE EFFECTS) Adding A, Adding B, Removing A, Adding A then updating B results in an incorrect batch configuration");


        }

    }
}
