using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Positioning;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.UtilityTypes.BindableProperties;
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
        public IBindableProperty<uint> TextureFile { get; }

        public IBindableProperty<double> TextureFileX { get; }

        public IBindableProperty<double> TextureFileY { get; }

        public IBindableProperty<double> TextureFileWidth { get; }

        public IBindableProperty<double> TextureFileHeight { get; }

        public IBindableProperty<IVector<float>> WorldPosition { get; } = new BindableProperty<IVector<float>>(new Vector<float>(0, 0, 0));

        public SpriteRenderObject(uint textureUint, double textureX, double textureY, double textureWidth, double textureHeight)
        {
            //When the vector changes, trigger change on the bindable property.
            WorldPosition.Value.OnChange += (object src, EventArgs arg) => { WorldPosition.TriggerChanged(); };
            //Set the bindable properties
            TextureFile = new BindableProperty<uint>(textureUint);
            TextureFileX = new BindableProperty<double>(textureX);
            TextureFileY = new BindableProperty<double>(textureY);
            TextureFileWidth = new BindableProperty<double>(textureWidth);
            TextureFileHeight = new BindableProperty<double>(textureHeight);
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
    }
}
