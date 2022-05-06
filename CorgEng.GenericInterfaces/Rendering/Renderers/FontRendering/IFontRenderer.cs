using CorgEng.GenericInterfaces.Rendering.RenderObjects.FontRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Renderers.FontRendering
{
    public interface IFontRenderer : IRenderer
    {

        void StartRendering(IFontRenderObject fontRenderObject);

        void StopRendering(IFontRenderObject fontRenderObject);

    }
}
