using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.UserInterface.Rendering.Renderer;
using CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject;
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
        private static IUserInterfaceRenderObjectFactory SpriteRenderObjectFactory;

        [UsingDependency]
        private static IUserInterfaceRendererFactory SpriteRendererFactory;

        private IUserInterfaceRenderer spriteRenderer;

        private IUserInterfaceRenderObject spriteRenderObject;

        public override void Initialize()
        {
            //Create the sprite renderer.
            spriteRenderer = SpriteRendererFactory.CreateUserInterfaceRenderer();
            //Initialize it
            spriteRenderer?.Initialize();

            spriteRenderObject = SpriteRenderObjectFactory.CreateUserInterfaceRenderObject(1, 0, 0, 256, 256);
            spriteRenderer.StartRendering(spriteRenderObject);
        }

        public override void PerformRender()
        {
            spriteRenderer?.Render(Core.CorgEngMain.MainCamera);
        }

    }
}
