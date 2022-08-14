using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.UtilityTypes.BindableProperties;
using CorgEng.UtilityTypes.Matrices;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer
{
    public class UserInterfaceRenderObject : IUserInterfaceRenderObject
    {

        public IBindableProperty<IMatrix> CombinedTransform { get; } = new BindableProperty<IMatrix>(new Matrix(new float[,] {
            { 1, 0, 0 },
            { 0, 1, 0 },
            //This last row is actually ignored (It's not needed in 2 dimensional applications)
            { 0, 0, 1 }
        }));

        public IBindableProperty<IVector<float>> CombinedTransformFirstRow { get; } = new BindableProperty<IVector<float>>(new Vector<float>(1, 0, 0));

        public IBindableProperty<IVector<float>> CombinedTransformSecondRow { get; } = new BindableProperty<IVector<float>>(new Vector<float>(0, 1, 0));

        public IBindablePropertyGroup TextureDetails { get; }

        public IEntityDefinition TypeDef { get; set; }

        public UserInterfaceRenderObject()
        {
            //When the vector changes, trigger change on the bindable property.
            CombinedTransform.Value.OnChange += (object src, EventArgs arg) => {
                //Trigger updates to our transform rows
                CombinedTransformFirstRow.Value.X = CombinedTransform.Value[1, 1];
                CombinedTransformFirstRow.Value.Y = CombinedTransform.Value[2, 1];
                CombinedTransformFirstRow.Value.Z = CombinedTransform.Value[3, 1];
                CombinedTransformFirstRow.TriggerChanged();
                CombinedTransformSecondRow.Value.X = CombinedTransform.Value[1, 2];
                CombinedTransformSecondRow.Value.Y = CombinedTransform.Value[2, 2];
                CombinedTransformSecondRow.Value.Z = CombinedTransform.Value[3, 2];
                CombinedTransformSecondRow.TriggerChanged();
            };
        }

        public IBatchElement<BatchType> GetBelongingBatchElement<BatchType>() where BatchType : IBatch<BatchType>
        {
            throw new NotImplementedException("User interface objects do not use batches: They are not instanced.");
        }

        public void SetBelongingBatchElement<BatchType>(IBatchElement<BatchType> heldBatch) where BatchType : IBatch<BatchType>
        {
            throw new NotImplementedException("User interface objects do not use batches: They are not instanced.");
        }

        public ISharedRenderAttributes GetSharedRenderAttributes()
        {
            throw new NotImplementedException("User interface objects do not use shared render attributes: They are not instanced.");
        }

    }
}
