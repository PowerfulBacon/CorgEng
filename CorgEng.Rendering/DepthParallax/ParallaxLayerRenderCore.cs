using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Renderers;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.Rendering.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.Rendering.DepthParallax
{
    public class ParallaxLayerRenderCore : SpriteRenderer
    {

        [UsingDependency]
        private static ISpriteRendererFactory SpriteRendererFactory = null!;

        /// <summary>
        /// The layer that this parallax renderer is on
        /// </summary>
        public int LayerValue { get; }

        /// <summary>
        /// The next layer in the sequence
        /// </summary>
        private ParallaxLayerRenderCore? next;

        /// <summary>
        /// Parallax render cores that are active in the world
        /// </summary>
        public static Dictionary<int, ParallaxLayerRenderCore[]> parallaxRenderCores = new Dictionary<int, ParallaxLayerRenderCore[]>();

        public override float DepthAdd => 0.01f;

        public ParallaxLayerRenderCore(IWorld world, int identifier, int layer) : base(world)
        {
            if (!parallaxRenderCores.ContainsKey(identifier))
                parallaxRenderCores.Add(identifier, new ParallaxLayerRenderCore[layer]);
            parallaxRenderCores[identifier][layer - 1] = this;
            LayerValue = layer;
            // Keep going until we reach layer 1
            if (layer == 1)
                return;
            next = new ParallaxLayerRenderCore(world, identifier, layer - 1);
        }

        public override void Initialize()
        {
            base.Initialize();
            next?.Initialize();
        }

        public override void Render(ICamera camera)
        {
            base.Render(camera);
            // Now render the layer below us
            if (next != null)
            {
                // TODO: Make the next layer not draw to the screen too
                next.Render(camera);
                next.DrawToBuffer(FrameBufferUint, 1, 1, Width, Height);
            }
        }

    }
}
