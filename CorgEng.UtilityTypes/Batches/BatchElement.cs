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
        private readonly IBindableProperty<IVector<float>>[] internalValues;

        private EventHandler[] eventHandlers;

        public BatchElement(IBindableProperty<IVector<float>>[] internalValues)
        {
            //Set the bindable properties
            this.internalValues = internalValues;
            //Bind the properties to update the batch when changed
            eventHandlers = new EventHandler[internalValues.Length];
            for (int i = 0; i < internalValues.Length; i++)
            {
                eventHandlers[i] = (object sender, EventArgs args) => {
                    OnBindablePropertyChanged(i, (IVector<float>)sender);
                };
                //Store the event handler, so it can be unbound later.
                internalValues[i].ValueChanged += eventHandlers[i];
            }
        }

        public void Unbind()
        {
            for (int i = 0; i < internalValues.Length; i++)
            {
                //Store the event handler, so it can be unbound later.
                internalValues[i].ValueChanged -= eventHandlers[i];
                //Nullify the event handler
                eventHandlers = null;
            }
        }

        private void OnBindablePropertyChanged(int index, IVector<float> newValue)
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
