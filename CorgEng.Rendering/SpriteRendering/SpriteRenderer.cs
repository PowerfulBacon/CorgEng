using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
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
using static OpenGL.Gl;

namespace CorgEng.Rendering.SpriteRendering
{
    public class SpriteRenderer : InstancedRenderer<ISpriteSharedRenderAttributes, SpriteBatch>, ISpriteRenderer
    {

        protected IShaderSet _shaderSet;

        protected override IShaderSet ShaderSet => _shaderSet;

		public SpriteRenderer(IWorld world) : base(world)
		{
		}

		protected override void CreateShaders()
        {
            _shaderSet = ShaderFactory.CreateShaderSet("SpriteShader");
        }

        public void StartRendering(ISpriteRenderObject spriteRenderObject)
        {
            //Check for duplicate render exceptions
            if (spriteRenderObject.GetBelongingBatchElement<SpriteBatch>() != null)
                throw new DuplicateRenderException("Attempting to render a sprite object already being rendered.");
            //Indicate that we are rendering
            spriteRenderObject.CurrentRenderer = this;
            //Create a new batch element for this
            IBatchElement<SpriteBatch> batchElement = new BatchElement<SpriteBatch>(new IBindableProperty<IVector<float>>[] {
                spriteRenderObject.CombinedTransformFirstRow,
                spriteRenderObject.CombinedTransformSecondRow,
                spriteRenderObject.TextureDetails,
                spriteRenderObject.IconLayer,
                spriteRenderObject.Colour
            });
            //Remember the batch element we are stored in, so it can be saved
            spriteRenderObject.SetBelongingBatchElement(batchElement);
            //Add to rendering cache
            AddToBatch(spriteRenderObject.GetSharedRenderAttributes(), batchElement);
        }

        public void StopRendering(ISpriteRenderObject spriteRenderObject)
        {
            if (spriteRenderObject.CurrentRenderer != this)
                throw new Exception("Attempted to stop rendering a render object thats not rendering on this renderer.");
            //Stop rendering
            spriteRenderObject.CurrentRenderer = null;
            //Remove references to the on change event
            spriteRenderObject.GetBelongingBatchElement<SpriteBatch>().Unbind();
            //Remove the batch element
            RemoveFromBatch(spriteRenderObject.GetSharedRenderAttributes(), spriteRenderObject.GetBelongingBatchElement<SpriteBatch>());
            spriteRenderObject.SetBelongingBatchElement<SpriteBatch>(null);
        }

        private int textureSamplerUniformLocation;

		protected override void LoadUniformVariableLocations()
        {
            base.LoadUniformVariableLocations();
            textureSamplerUniformLocation = glGetUniformLocation(programUint, "renderTexture");
        }

        protected override void BindUniformVariables(ICamera camera)
        {
            base.BindUniformVariables(camera);
            glUniform1i(textureSamplerUniformLocation, 0);
        }

        protected override void BindBatchTexture(ISpriteSharedRenderAttributes batchAttributes)
        {
            //Bind the texture
            glActiveTexture(GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_2D, batchAttributes.SpriteTextureUint);
        }
    }
}
