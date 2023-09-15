using CorgEng.Core;
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

        /// <summary>
        /// Cached position of the component, so that if the position gets set before
        /// we start rendering, we can be rendered in the correct position.
        /// </summary>
        public IVector<float> CachedPosition { get; set; } = null;

        /// <summary>
        /// Same as above but for rotation
        /// </summary>
        public IVector<float> CachedRotation { get; set; } = null!;

        /// <summary>
        /// The sprite to be drawn by this component
        /// </summary>
        [NetworkSerialized]
        public IIcon Sprite { get; set; }

        /// <summary>
        /// The overlays to apply on boot
        /// </summary>
        public IIcon[] InitialOverlays { get; set; }

        /// <summary>
        /// The render object that is drawing this component
        /// </summary>
        public ISpriteRenderObject SpriteRenderObject { get; internal set; }

        /// <summary>
        /// The identifier of the sprite renderer to use
        /// </summary>
        [NetworkSerialized]
        public int SpriteRendererIdentifier { get; set; }

        /// <summary>
        /// Is the sprite currently being rendered
        /// </summary>
        internal bool IsRendering { get; set; } = false;

        internal bool WantsToRender { get; set; } = true;

        private int cachedSpriteRendererIdentifier = 0;

        protected ISpriteRenderer _spriteRenderer;

        public virtual ISpriteRenderer SpriteRenderer
        {
            get
            {
                if (SpriteRendererIdentifier == 0)
                    return null;
                if (_spriteRenderer == null || cachedSpriteRendererIdentifier != SpriteRendererIdentifier)
                {
                    _spriteRenderer = (ISpriteRenderer)CorgEngMain.GetRendererForPlane(SpriteRendererIdentifier);
                    cachedSpriteRendererIdentifier = SpriteRendererIdentifier;
                }
                return _spriteRenderer;
            }
        }

    }
}
