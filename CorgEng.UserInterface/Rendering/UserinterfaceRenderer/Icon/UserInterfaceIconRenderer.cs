using CorgEng.GenericInterfaces.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer.Icon
{
    internal class UserInterfaceIconRenderer : UserInterfaceRenderer<UserInterfaceIconRenderObject>
    {

        protected override IShaderSet ShaderSet { get; } = ShaderFactory.CreateShaderSet("UIIconShader");

        private int uniformLocationSprite;
        private int uniformTextureOffset;

        protected override void FetchUniformVariableLocations()
        {
            uniformLocationSprite = glGetUniformLocation(programUint, "sampler");
            uniformTextureOffset = glGetUniformLocation(programUint, "iconOffset");
        }

        protected override void BindUniformLocations(UserInterfaceIconRenderObject renderObject)
        {
            glActiveTexture(GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_2D, renderObject.Texture.TextureFile.TextureID);
            glUniform1i(uniformLocationSprite, 0);
            glUniform4f(uniformTextureOffset, renderObject.Texture.OffsetX, renderObject.Texture.OffsetY, renderObject.Texture.OffsetWidth, renderObject.Texture.OffsetHeight);
        }

    }
}
