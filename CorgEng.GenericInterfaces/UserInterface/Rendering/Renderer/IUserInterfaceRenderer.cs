using CorgEng.GenericInterfaces.Rendering.Renderers;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Rendering.Renderer
{
    public interface IUserInterfaceRenderer<T>
        where T : IUserInterfaceRenderObject
    {

        void StartRendering(T spriteRenderObject);

        void StopRendering(T spriteRenderObject);

        void Initialize();

        void Render(int pixelWidth, int pixelHeight);

    }
}
