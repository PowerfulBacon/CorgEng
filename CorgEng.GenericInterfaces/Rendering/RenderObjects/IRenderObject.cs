using CorgEng.GenericInterfaces.ContentLoading;
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
    public interface IRenderObject : IInstantiatable
    {

        /// <summary>
        /// The transform applied to this render object.
        /// Used externally, will trigger updates to the transform rows automatically.
        /// </summary>
        IBindableProperty<IMatrix> Transform { get; }

        /// <summary>
        /// The first row of the transfomation matrix.
        /// Updated internally by the renderers and shouldn't be interfaced with directly.
        /// </summary>
        IBindableProperty<IVector<float>> TransformFirstRow { get; }

        /// <summary>
        /// The second row of the transfomation matrix.
        /// Updated internally by the renderers and shouldn't be interfaced with directly.
        /// </summary>
        IBindableProperty<IVector<float>> TransformSecondRow { get; }

        void SetBelongingBatchElement<BatchType>(IBatchElement<BatchType> heldBatch)
            where BatchType : IBatch<BatchType>;

        IBatchElement<BatchType> GetBelongingBatchElement<BatchType>()
            where BatchType : IBatch<BatchType>;

        ISharedRenderAttributes GetSharedRenderAttributes();

    }
}
