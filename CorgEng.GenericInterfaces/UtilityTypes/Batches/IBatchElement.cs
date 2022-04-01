namespace CorgEng.GenericInterfaces.UtilityTypes.Batches
{
    public interface IBatchElement<TargetBatch>
        where TargetBatch : IBatch<TargetBatch>
    {
        //The batch that this element belongs to, if any
        IBatch<TargetBatch> ContainingBatch { get; set; }

        //The position of this element in the batch
        int BatchPosition { get; set; }

        //Get the value at a specific index
        IVector<float> GetValue(int index);

        //Called when removed from a render batch, unbinds the properties.
        void Unbind();
    }
}
