using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Renderers.ParallaxRenderer
{
    public interface IParallaxRendererFactory
    {

        /// <summary>
        /// Create a parallax renderer
        /// </summary>
        /// <param name="networkedIdentifier"></param>
        /// <returns></returns>
        IParallaxRenderer CreateParallaxRenderer(uint networkedIdentifier);

    }
}
