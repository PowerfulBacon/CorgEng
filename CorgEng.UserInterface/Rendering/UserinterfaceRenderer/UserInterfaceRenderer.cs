using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using CorgEng.GenericInterfaces.UserInterface.Rendering.Renderer;
using CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using CorgEng.Rendering;
using CorgEng.Rendering.Exceptions;
using CorgEng.UtilityTypes.Batches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer
{
    internal sealed class UserInterfaceRenderer : InstancedRenderer<UserInterfaceSharedRenderAttributes, UserInterfaceBatch>, IUserInterfaceRenderer
    {

        [UsingDependency]
        private static IShaderFactory ShaderFactory;

        private IShaderSet _shaderSet;
        protected override IShaderSet ShaderSet => _shaderSet;

        protected override void CreateShaders()
        {
            _shaderSet = ShaderFactory.CreateShaderSet("UserInterfaceShader");
        }

        public void StartRendering(IUserInterfaceRenderObject spriteRenderObject)
        {
            //Check for duplicate render exceptions
            if (spriteRenderObject.GetBelongingBatchElement<UserInterfaceBatch>() != null)
                throw new DuplicateRenderException("Attempting to render a sprite object already being rendered.");
            //Create a new batch element for this
            IBatchElement<UserInterfaceBatch> batchElement = new BatchElement<UserInterfaceBatch>(new IBindableProperty<IVector<float>>[] {
                spriteRenderObject.WorldPosition,
                spriteRenderObject.TextureDetails
            });
            //Remember the batch element we are stored in, so it can be saved
            spriteRenderObject.SetBelongingBatchElement(batchElement);
            //Add to rendering cache
            AddToBatch(spriteRenderObject.GetSharedRenderAttributes(), batchElement);
        }

        public void StopRendering(IUserInterfaceRenderObject spriteRenderObject)
        {
            //Remove references to the on change event
            spriteRenderObject.GetBelongingBatchElement<UserInterfaceBatch>().Unbind();
            //Remove the batch element
            RemoveFromBatch(spriteRenderObject.GetSharedRenderAttributes(), spriteRenderObject.GetBelongingBatchElement<UserInterfaceBatch>());
            spriteRenderObject.SetBelongingBatchElement<UserInterfaceBatch>(null);
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

        protected override void BindBatchTexture(UserInterfaceSharedRenderAttributes batchAttributes)
        {
            //Bind the texture
            glActiveTexture(GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_2D, batchAttributes.SpriteTextureUint);
        }

    }
}
