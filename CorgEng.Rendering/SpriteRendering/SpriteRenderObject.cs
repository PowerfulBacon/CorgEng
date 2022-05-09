using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Rendering.Positioning;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
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

namespace CorgEng.Rendering.SpriteRendering
{
    public sealed class SpriteRenderObject : ISpriteRenderObject
    {
        public IBindableProperty<uint> TextureFile { get; set; }

        public IBindableProperty<float> TextureFileX { get; set; }

        public IBindableProperty<float> TextureFileY { get; set; }

        public IBindableProperty<float> TextureFileWidth { get; set; }

        public IBindableProperty<float> TextureFileHeight { get; set; }

        public IBindablePropertyGroup TextureDetails { get; }

        public IEntityDef TypeDef { get; set; }

        public IBindableProperty<IMatrix> Transform { get; } = new BindableProperty<IMatrix>(new Matrix(new float[,] {
            { 1, 0, 0 },
            { 0, 1, 0 },
            //This last row is actually ignored
            { 0, 0, 1 }
        }));

        public IBindableProperty<IVector<float>> TransformFirstRow { get; } = new BindableProperty<IVector<float>>(new Vector<float>(1, 0, 0));

        public IBindableProperty<IVector<float>> TransformSecondRow { get; } = new BindableProperty<IVector<float>>(new Vector<float>(0, 1, 0));

        public SpriteRenderObject(uint textureUint, float textureX, float textureY, float textureWidth, float textureHeight)
        {
            //When the vector changes, trigger change on the bindable property.
            Transform.Value.OnChange += (object src, EventArgs arg) => {
                //Trigger updates to our transform rows
                TransformFirstRow.Value.X = Transform.Value[1, 1];
                TransformFirstRow.Value.Y = Transform.Value[2, 1];
                TransformFirstRow.Value.Z = Transform.Value[3, 1];
                TransformFirstRow.TriggerChanged();
                TransformSecondRow.Value.X = Transform.Value[1, 2];
                TransformSecondRow.Value.Y = Transform.Value[2, 2];
                TransformSecondRow.Value.Z = Transform.Value[3, 2];
                TransformSecondRow.TriggerChanged();
                //Trigger a transform update
                Transform.TriggerChanged();
            };
            //Set the bindable properties
            TextureFile = new BindableProperty<uint>(textureUint);
            TextureFileX = new BindableProperty<float>(textureX);
            TextureFileY = new BindableProperty<float>(textureY);
            TextureFileWidth = new BindableProperty<float>(textureWidth);
            TextureFileHeight = new BindableProperty<float>(textureHeight);
            TextureDetails = new BindablePropertyGroup(TextureFileX, TextureFileY, TextureFileWidth, TextureFileHeight);
        }

        private object batchContained;

        public IBatchElement<BatchType> GetBelongingBatchElement<BatchType>() where BatchType : IBatch<BatchType>
        {
            return (IBatchElement<BatchType>)batchContained;
        }

        public void SetBelongingBatchElement<BatchType>(IBatchElement<BatchType> heldBatch) where BatchType : IBatch<BatchType>
        {
            batchContained = heldBatch;
        }

        public ISharedRenderAttributes GetSharedRenderAttributes()
        {
            return new SpriteSharedRenderAttributes(TextureFile.Value);
        }

        public void PreInitialize(IVector<float> initializePosition)
        { }

        public void Initialize(IVector<float> initializePosition)
        { }

        public bool SetProperty(string name, IPropertyDef property)
        {
            //TODO: Textures can be loaded from the def file
            return false;
        }

    }
}
