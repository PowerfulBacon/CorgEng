using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    [Dependency(defaultDependency = true)]
    public class SpriteRenderObjectFactory : ISpriteRenderObjectFactory
    {

        public ISpriteRenderObject CreateSpriteRenderObject(uint textureFile, double textureX, double textureY, double textureWidth, double textureHeight)
        {
            return new SpriteRenderObject(textureFile, textureX, textureY, textureWidth, textureHeight);
        }

    }
}
