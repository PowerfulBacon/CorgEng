using CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.Rendering.DepthParallax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.ComponentSystem.DepthParallax
{
    public class DepthSpriteRenderComponent : SpriteRenderComponent
    {

        public override ISpriteRenderer SpriteRenderer
        {
            get
            {
                if (SpriteRendererIdentifier == 0)
                    return null;
                if (_spriteRenderer == null || cachedSpriteRendererIdentifier != SpriteRendererIdentifier)
                {
                    if (!ParallaxLayerRenderCore.parallaxRenderCores.ContainsKey((int)SpriteRendererIdentifier))
                        return null;
                    _spriteRenderer = ParallaxLayerRenderCore.parallaxRenderCores[(int)SpriteRendererIdentifier][(int)Sprite.Layer-1];
                    cachedSpriteRendererIdentifier = SpriteRendererIdentifier;
                }
                return _spriteRenderer;
            }
        }

    }
}
