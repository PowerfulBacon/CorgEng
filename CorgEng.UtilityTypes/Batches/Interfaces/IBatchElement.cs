using CorgEng.UtilityTypes.Vectors;

namespace CorgEng.UtilityTypes.Batches.Interfaces
{
    public interface IBatchElement<TargetBatch>
        where TargetBatch : IBatch<TargetBatch>
    {
        //The batch that this element belongs to, if any
        IBatch<TargetBatch> ContainingBatch { get; set; }

        //The position of this element in the batch
        int BatchPosition { get; set; }

        //Get the value at a specific index
        Vector<float> GetValue(int index);
    }
}
