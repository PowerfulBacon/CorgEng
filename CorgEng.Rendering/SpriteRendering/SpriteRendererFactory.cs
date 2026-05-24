using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering
{
    [Dependency]
    public class SpriteRendererFactory : ISpriteRendererFactory
    {

        public ISpriteRenderer CreateSpriteRenderer(IWorld world, int plane)
        {
            var spriteRenderer = new SpriteRenderer(world);
            return spriteRenderer;
		}

    }
}
