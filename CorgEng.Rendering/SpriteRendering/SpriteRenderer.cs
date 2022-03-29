using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.Rendering.Exceptions;
using CorgEng.UtilityTypes.Batches;
using CorgEng.UtilityTypes.BindableProperties;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    [Dependency(defaultDependency = true)]
    internal sealed class SpriteRenderer : InstancedRenderer<SpriteSharedRenderAttributes, SpriteBatch>, ISpriteRenderer
    {

        protected override IShaderSet ShaderSet { get; }

        public void StartRendering(ISpriteRenderObject spriteRenderObject)
        {
            //Check for duplicate render exceptions
            if (spriteRenderObject.GetBelongingBatchElement<SpriteBatch>() != null)
                throw new DuplicateRenderException("Attempting to render a sprite object already being rendered.");
            //Create a new batch element for this
            IBatchElement<SpriteBatch> batchElement = new BatchElement<SpriteBatch>(new IBindableProperty<IVector<float>>[] {
                spriteRenderObject.WorldPosition
            });
            //Remember the batch element we are stored in, so it can be saved
            spriteRenderObject.SetBelongingBatchElement(batchElement);
            //Add to rendering cache
            AddToBatch(spriteRenderObject.GetSharedRenderAttributes(), batchElement);
        }

        public void StopRendering(ISpriteRenderObject spriteRenderObject)
        {
            //Remove references to the on change event
            spriteRenderObject.GetBelongingBatchElement<SpriteBatch>().Unbind();
            //Remove the batch element
            RemoveFromBatch(spriteRenderObject.GetSharedRenderAttributes(), spriteRenderObject.GetBelongingBatchElement<SpriteBatch>());
            spriteRenderObject.SetBelongingBatchElement<SpriteBatch>(null);
        }

        protected override void BindBatchAttributes(SpriteSharedRenderAttributes sharedRenderAttributes, SpriteBatch batch)
        {
            throw new NotImplementedException();
        }

        protected override void BindUniformVariables()
        {
            throw new NotImplementedException();
        }

        protected override void LoadUniformVariableLocations()
        {
            throw new NotImplementedException();
        }
    }
}
