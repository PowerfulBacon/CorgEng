using CorgEng.GenericInterfaces.Rendering.Renderers;
using CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Rendering.Renderer
{
    public interface IUserInterfaceRenderer : IRenderer
    {

        void StartRendering(IUserInterfaceRenderObject userInterfaceRenderObject);

        void StopRendering(IUserInterfaceRenderObject userInterfaceRenderObject);

    }
}
