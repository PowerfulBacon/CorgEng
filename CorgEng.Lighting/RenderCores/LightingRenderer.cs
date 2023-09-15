using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.Rendering.SpriteRendering;
using static OpenGL.Gl;

namespace CorgEng.Lighting.RenderCores
{
    public class LightingRenderer : SpriteRenderer
    {

        public static int LIGHTING_PLANE = 10;

        public override RenderModes DrawMode => RenderModes.MULTIPLY;

        public override RenderModes BlendMode => RenderModes.ADDITIVE;

        public override DepthModes DepthMode => DepthModes.IGNORE_DEPTH;

        public ISpriteRenderer lightRenderer = null!;

        public override IColour BackColour => ColourFactory.GetColour(0.4f, 0.4f, 0.4f, 1);

    }
}
