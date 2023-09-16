using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering.Renderers.ParallaxRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering.Parallax
{
    [Dependency]
    internal class ParallaxRendererFactory : IParallaxRendererFactory
    {
        public IParallaxRenderer CreateParallaxRenderer(IWorld world, int plane)
        {
            return new ParallaxRenderer(world);
        }
    }
}
