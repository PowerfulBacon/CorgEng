using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering
{
    public interface ISpriteRenderer : IRenderer
    {

        void StartRendering(ISpriteRenderObject spriteRenderObject);

        void StopRendering(ISpriteRenderObject spriteRenderObject);

    }
}
