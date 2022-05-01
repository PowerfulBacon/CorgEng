using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Rendering
{
    internal class UserInterfaceRenderCore : RenderCore
    {

        [UsingDependency]
        private static ISpriteRenderObjectFactory SpriteRenderObjectFactory;

        [UsingDependency]
        private static ISpriteRendererFactory SpriteRendererFactory;

        private ISpriteRenderer spriteRenderer;

        public override void Initialize()
        {
            //Create the sprite renderer.
            spriteRenderer = SpriteRendererFactory.CreateSpriteRenderer();
            //Initialize it
            spriteRenderer?.Initialize();
        }

        public override void PerformRender()
        {
            
        }

    }
}
