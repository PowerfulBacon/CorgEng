using CorgEng.GenericInterfaces.Rendering.Positioning;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.RenderObjects
{
    public interface IRenderObject
    {

        IBindableProperty<IVector<float>> WorldPosition { get; }

        void SetBelongingBatchElement<BatchType>(IBatchElement<BatchType> heldBatch)
            where BatchType : IBatch<BatchType>;

        IBatchElement<BatchType> GetBelongingBatchElement<BatchType>()
            where BatchType : IBatch<BatchType>;

        ISharedRenderAttributes GetSharedRenderAttributes();

    }
}
