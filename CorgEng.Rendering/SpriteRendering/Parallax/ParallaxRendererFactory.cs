using CorgEng.DependencyInjection.Dependencies;
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
        public IParallaxRenderer CreateParallaxRenderer(int plane)
        {
            return new ParallaxRenderer();
        }
    }
}
