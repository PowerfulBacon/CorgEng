using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.UtilityTypes;
using static OpenGL.Gl;

namespace CorgEng.Lighting.RenderCores
{
    public class LightingRenderCore : RenderCore
    {

        public const int LIGHTING_PLANE = 10;

        [UsingDependency]
        private static ISpriteRendererFactory SpriteRendererFactory = default!;

        private static LightingRenderCore? _singleton;
        public static LightingRenderCore Singleton
        {
            get {
                if (_singleton == null)
                {
                    _singleton = new LightingRenderCore();
                    _singleton.Initialize();
                }
                return _singleton;
            }
        }

        public static void Resolve()
        {
            if (_singleton == null)
            {
                _singleton = new LightingRenderCore();
                _singleton.Initialize();
            }
        }

        public override RenderModes DrawMode => RenderModes.MULTIPLY;

        public override RenderModes BlendMode => RenderModes.ADDITIVE;

        public override DepthModes DepthMode => DepthModes.IGNORE_DEPTH;

        public ISpriteRenderer lightRenderer = null!;

        public override IColour BackColour => ColourFactory.GetColour(0.4f, 0.4f, 0.4f, 1);

        public override void Initialize()
        {
            lightRenderer = SpriteRendererFactory.CreateSpriteRenderer(LIGHTING_PLANE);
            lightRenderer.Initialize();
        }

        public override void PerformRender()
        {
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            lightRenderer.Render(CorgEngMain.MainCamera);
        }

    }
}
