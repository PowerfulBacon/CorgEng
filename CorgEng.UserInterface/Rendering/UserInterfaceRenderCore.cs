using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Text;
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
        private static IUserInterfaceRenderObjectFactory UserInterfaceRenderObjectFactory;

        [UsingDependency]
        private static IUserInterfaceRendererFactory UserInterfaceRendererFactory;

        [UsingDependency]
        private static IFontFactory FontFactory;

        [UsingDependency]
        private static ITextObjectFactory TextObjectFactory;

        private IUserInterfaceRenderer userInterfaceRenderer;

        private IUserInterfaceRenderObject spriteRenderObject;

        public override void Initialize()
        {
            //Create the sprite renderer.
            userInterfaceRenderer = UserInterfaceRendererFactory.CreateUserInterfaceRenderer();
            //Initialize it
            userInterfaceRenderer?.Initialize();

            spriteRenderObject = UserInterfaceRenderObjectFactory.CreateUserInterfaceRenderObject(1, 0, 0, 256, 256);
            userInterfaceRenderer.StartRendering(spriteRenderObject);

            IFont font = FontFactory.GetFont("CourierCode");
            ITextObject textObject = TextObjectFactory.CreateTextObject(userInterfaceRenderer, font, "CorgEng.Font");
            textObject.StartRendering();
        }

        public override void PerformRender()
        {
            userInterfaceRenderer?.Render(Core.CorgEngMain.MainCamera);
        }

    }
}
