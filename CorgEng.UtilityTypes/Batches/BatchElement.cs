using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.UtilityTypes.BindableProperties;
using CorgEng.UtilityTypes.Vectors;
using System;

namespace CorgEng.UtilityTypes.Batches
{
    public class BatchElement<TargetBatch> : IBatchElement<TargetBatch>
        where TargetBatch : IBatch<TargetBatch>
    {

        //The batch we are contained withing
        public IBatch<TargetBatch> ContainingBatch { get; set; }

        //The index we are stored at
        public int BatchPosition { get; set; }

        //The values this batch element contains
        //Note that vectors are stored by value not by type
        private readonly BindableProperty<Vector<float>>[] internalValues;

        public BatchElement(BindableProperty<Vector<float>>[] internalValues)
        {
            //Set the bindable properties
            this.internalValues = internalValues;
            //Bind the properties to update the batch when changed
            for (int i = 0; i < internalValues.Length; i++)
            {
                internalValues[i].ValueChanged += (object sender, EventArgs args) => {
                    OnBindablePropertyChanged(i, (Vector<float>)sender);
                };
            }
        }

        private void OnBindablePropertyChanged(int index, Vector<float> newValue)
        {
            //Not contained within any parent batch
            if (ContainingBatch == null)
                return;
            //Update ourselves in the batch
            ContainingBatch.Update(BatchPosition, index, newValue);
        }

        public IVector<float> GetValue(int index)
        {
            return internalValues[index].Value;
        }
    }
}
