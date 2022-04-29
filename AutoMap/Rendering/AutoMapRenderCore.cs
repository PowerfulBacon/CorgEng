using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMap.Rendering
{
    public class AutoMapRenderCore : RenderCore
    {

        [UsingDependency]
        public static ISpriteRenderer spriteRenderer;

        public override void Initialize()
        {
            spriteRenderer?.Initialize();
        }

        public override void PerformRender()
        {
            spriteRenderer?.Render(CorgEngMain.MainCamera);
        }

    }
}
