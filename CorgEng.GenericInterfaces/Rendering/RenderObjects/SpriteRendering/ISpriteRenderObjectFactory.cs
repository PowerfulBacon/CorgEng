using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering
{
    public interface ISpriteRenderObjectFactory
    {

        ISpriteRenderObject CreateSpriteRenderObject(uint textureFile, double textureX, double textureY, double textureWidth, double textureHeight);

    }
}
