using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.Rendering;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class SpriteRenderComponent : Component, IInstantiatable
    {

        public IVector<float> CachedPosition { get; set; } = null;

        [NetworkSerialized]
        public IIcon Sprite { get; set; }

        public ISpriteRenderObject SpriteRenderObject { get; internal set; }

        [NetworkSerialized]
        public uint SpriteRendererIdentifier { get; set; }

        private uint cachedSpriteRendererIdentifier = 0;

        private ISpriteRenderer _spriteRenderer;

        public ISpriteRenderer SpriteRenderer
        {
            get
            {
                if (SpriteRendererIdentifier == 0)
                    return null;
                if (_spriteRenderer == null || cachedSpriteRendererIdentifier != SpriteRendererIdentifier)
                {
                    _spriteRenderer = RendererLookup.GetRendererByIdentifier<ISpriteRenderer>(SpriteRendererIdentifier);
                    cachedSpriteRendererIdentifier = SpriteRendererIdentifier;
                }
                return _spriteRenderer;
            }
        }

    }
}
