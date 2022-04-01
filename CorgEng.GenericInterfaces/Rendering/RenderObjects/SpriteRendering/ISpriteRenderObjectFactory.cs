using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering
{
    public interface ISpriteRenderObjectFactory
    {

        ISpriteRenderObject CreateSpriteRenderObject(uint textureFile, float textureX, float textureY, float textureWidth, float textureHeight);

    }
}
