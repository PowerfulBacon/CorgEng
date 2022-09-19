using CorgEng.GenericInterfaces.Rendering.Renderers.ParallaxRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.SpriteRendering.Parallax
{
    internal class ParallaxRenderer : SpriteRenderer, IParallaxRenderer
    {
        internal ParallaxRenderer(uint networkedId) : base(networkedId)
        { }

        protected override void CreateShaders()
        {
            _shaderSet = ShaderFactory.CreateShaderSet("ParallaxShader");
        }

    }
}
