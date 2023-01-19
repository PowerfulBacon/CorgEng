using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Positioning;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using CorgEng.GenericInterfaces.Rendering.Textures;
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

        [UsingDependency]
        private static ITextureFactory TextureFactory;

        [UsingDependency]
        private static ISpriteRenderObjectFactory SpriteRenderObjectFactory;

        public IBindableProperty<uint> TextureFile { get; set; }

        public IBindableProperty<float> TextureFileX { get; set; }

        public IBindableProperty<float> TextureFileY { get; set; }

        public IBindableProperty<float> TextureFileWidth { get; set; }

        public IBindableProperty<float> TextureFileHeight { get; set; }

        public IBindablePropertyGroup TextureDetails { get; }

        public IEntityDefinition TypeDef { get; set; }

        public IBindableProperty<IMatrix> CombinedTransform { get; } = new BindableProperty<IMatrix>(new Matrix(new float[,] {
            { 1, 0, 0 },
            { 0, 1, 0 },
            //This last row is actually ignored
            { 0, 0, 1 }
        }));

        public IBindableProperty<IVector<float>> CombinedTransformFirstRow { get; } = new BindableProperty<IVector<float>>(new Vector<float>(1, 0, 0));

        public IBindableProperty<IVector<float>> CombinedTransformSecondRow { get; } = new BindableProperty<IVector<float>>(new Vector<float>(0, 1, 0));

        private ISpriteRenderer _currentRenderer;
        public ISpriteRenderer CurrentRenderer
        {
            get => _currentRenderer;
            set
            {
                //Set the renderer
                _currentRenderer = value;
            }
        }

        public ISpriteRenderObject Container { get; set; }

        public IBindableProperty<IMatrix> SelfTransform { get; set; } = new BindableProperty<IMatrix>(new Matrix(new float[,] {
            { 1, 0, 0 },
            { 0, 1, 0 },
            //This last row is actually ignored
            { 0, 0, 1 }
        }));

        /// <summary>
        /// The rendering layer
        /// </summary>
        public IBindableProperty<IVector<float>> IconLayer { get; set; } = new BindableProperty<IVector<float>>(new Vector<float>(0));

        /// <summary>
        /// The RGBA colour to multiply this sprite by.
        /// </summary>
        public IBindableProperty<IVector<float>> Colour { get; set; } = new BindableProperty<IVector<float>>(new Vector<float>(1, 1, 1, 1));

        /// <summary>
        /// A hashset containing all overlays that are currently attached to us
        /// </summary>
        private Dictionary<IIcon, ISpriteRenderObject> overlays = new Dictionary<IIcon, ISpriteRenderObject>();

        public SpriteRenderObject(uint textureUint, float textureX, float textureY, float textureWidth, float textureHeight, float layer)
        {
            //Set the layer
            IconLayer.Value[0] = layer;
            //When the vector changes, trigger change on the bindable property.
            CombinedTransform.Value.OnChange += (object src, EventArgs arg) =>
            {
                //Trigger a transform update
                CombinedTransform.TriggerChanged();
            };
            //When the value of the combined transform is changed, we need to update our
            //rows, as they are bound to by the renderer which is where the updating needs
            //to be done.
            CombinedTransform.ValueChanged += (object src, EventArgs arg) =>
            {
                //Trigger updates to our transform rows
                CombinedTransformFirstRow.Value.X = CombinedTransform.Value[1, 1];
                CombinedTransformFirstRow.Value.Y = CombinedTransform.Value[2, 1];
                CombinedTransformFirstRow.Value.Z = CombinedTransform.Value[3, 1];
                CombinedTransformFirstRow.TriggerChanged();
                CombinedTransformSecondRow.Value.X = CombinedTransform.Value[1, 2];
                CombinedTransformSecondRow.Value.Y = CombinedTransform.Value[2, 2];
                CombinedTransformSecondRow.Value.Z = CombinedTransform.Value[3, 2];
                CombinedTransformSecondRow.TriggerChanged();
                //Update overlays transforms.
                foreach (ISpriteRenderObject overlayObject in overlays.Values)
                {
                    overlayObject.CombinedTransform.Value = CombinedTransform.Value.Multiply(overlayObject.SelfTransform.Value);
                }
            };
            //When the self transform is updated, the combined transform needs to be updated too
            SelfTransform.Value.OnChange += (object src, EventArgs arg) =>
            {
                SelfTransform.TriggerChanged();
            };
            //When the self transform is updated, calculate and apply our combined transform
            SelfTransform.ValueChanged += (object src, EventArgs args) =>
            {
                CombinedTransform.Value = Container != null
                    ? Container.CombinedTransform.Value.Multiply(SelfTransform.Value)
                    : SelfTransform.Value;
            };
            //Set the bindable properties
            TextureFile = new BindableProperty<uint>(textureUint);
            TextureFileX = new BindableProperty<float>(textureX);
            TextureFileY = new BindableProperty<float>(textureY);
            TextureFileWidth = new BindableProperty<float>(textureWidth);
            TextureFileHeight = new BindableProperty<float>(textureHeight);
            TextureDetails = new BindablePropertyGroup(TextureFileX, TextureFileY, TextureFileWidth, TextureFileHeight);
        }

        public void RedrawFromIcon(IIcon sourceIcon)
        {
            //Set the colour
            Colour.Value = sourceIcon.Colour;
            //Set the layer
            IconLayer.Value[0] = sourceIcon.Layer;
            IconLayer.TriggerChanged();
            // Change the renderer if required
            if (CurrentRenderer != sourceIcon.Renderer)
            {
                //Stop drawing
                CurrentRenderer?.StopRendering(this);
                //Start drawing
                sourceIcon.Renderer?.StartRendering(this);
            }
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

        public void AddOverlay(IIcon overlay)
        {
            if (overlay.Renderer == null)
                throw new ArgumentNullException("An icon will a null renderer was added as an overlay.");
            //Get the texture details
            ITextureState textureState = TextureFactory.GetTextureFromIconState(overlay);
            //Get the sprite render object
            ISpriteRenderObject spriteRenderObject = SpriteRenderObjectFactory.CreateSpriteRenderObject(
                textureState.TextureFile.TextureID,
                textureState.OffsetX,
                textureState.OffsetY,
                textureState.OffsetWidth,
                textureState.OffsetHeight,
                IconLayer.Value[0] + 0.0001f);
            spriteRenderObject.RedrawFromIcon(overlay);
            overlay.ValueChanged += () => spriteRenderObject.RedrawFromIcon(overlay);
            //Set it as the container
            spriteRenderObject.Container = this;
            //If we are currently being rendered, render the overlay on the same renderer
            //overlay.Renderer?.StartRendering(spriteRenderObject);
            //Copy across the transform
            spriteRenderObject.SelfTransform.Value = overlay.Transform;
            //Add the overlay
            overlays.Add(
                overlay,
                spriteRenderObject
                );
        }

        public void RemoveOverlay(IIcon overlay)
        {
            //Stop rendering the overlay
            overlay.Renderer?.StopRendering(overlays[overlay]);
            //Remove the overlay
            overlays.Remove(overlay);
        }

    }
}
