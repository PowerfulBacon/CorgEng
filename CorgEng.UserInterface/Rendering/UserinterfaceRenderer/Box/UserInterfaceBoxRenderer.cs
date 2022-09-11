using CorgEng.GenericInterfaces.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer.Box
{
    internal class UserInterfaceBoxRenderer : UserInterfaceRenderer<UserInterfaceBoxRenderObject>
    {
        protected override IShaderSet ShaderSet { get; } = ShaderFactory.CreateShaderSet("BoxShader");

        private int UniformLocationBorderWidth;
        private int UniformLocationBorderColour;
        private int UniformLocationFillColour;

        protected override void FetchUniformVariableLocations()
        {
            UniformLocationBorderWidth = glGetUniformLocation(programUint, "uniformLocationBorderWidth");
            UniformLocationBorderColour = glGetUniformLocation(programUint, "uniformLocationBorderColour");
            UniformLocationFillColour = glGetUniformLocation(programUint, "uniformLocationFillColour");
        }

        protected override void BindUniformLocations(UserInterfaceBoxRenderObject userInterfaceBoxRenderObject)
        {
            glUniform1f(UniformLocationBorderWidth, userInterfaceBoxRenderObject.Style.BorderWidth);
            glUniform4f(UniformLocationBorderColour, userInterfaceBoxRenderObject.Style.BorderColour.Red, userInterfaceBoxRenderObject.Style.BorderColour.Green, userInterfaceBoxRenderObject.Style.BorderColour.Blue, userInterfaceBoxRenderObject.Style.BorderColour.Alpha);
            glUniform4f(UniformLocationFillColour, userInterfaceBoxRenderObject.Style.FillColour.Red, userInterfaceBoxRenderObject.Style.FillColour.Green, userInterfaceBoxRenderObject.Style.FillColour.Blue, userInterfaceBoxRenderObject.Style.FillColour.Alpha);
        }



    }
}
