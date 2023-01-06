using AutoMap.Parser;
using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Generators;
using CorgEng.GenericInterfaces.UserInterface.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMap.Rendering
{
    public class AutoMapRenderCore : RenderCore
    {

        public static AutoMapRenderCore Singleton;

        [UsingDependency]
        public static ISpriteRendererFactory SpriteRendererFactory;

        [UsingDependency]
        private static IPopupManager PopupManager = null!;

        [UsingDependency]
        private static IUserInterfaceXmlLoader UserInterfaceXmlLoader = null!;

        private ISpriteRenderer spriteRenderer;

        public static IPopupWindow OpenPopup;

        private static IUserInterfaceComponent? mainScreenUserInterface;

        public static ParsedEnvironment openEnvironment;

        public AutoMapRenderCore()
        {
            Singleton = this;
        }

        public override void Initialize()
        {
            spriteRenderer = SpriteRendererFactory.CreateSpriteRenderer(0);
            spriteRenderer?.Initialize();
            OpenPopup = PopupManager.DisplayPopup(UserInterfaceXmlLoader.LoadUserInterface("Content/UserInterface/AutoMapMenu.xml"), Width / 2 - 400, Height / 2 - 300, 800, 600);
        }

        public override void PerformRender()
        {
            spriteRenderer?.Render(CorgEngMain.MainCamera);
            mainScreenUserInterface?.DrawToFramebuffer(FrameBufferUint);
            PopupManager.RenderToFramebuffer(FrameBufferUint);
        }

        public void OpenEnvironment(ParsedEnvironment parsedEnvironment)
        {
            openEnvironment = parsedEnvironment;
            //Close the load menu popup
            OpenPopup?.ClosePopup();
            //Open the environment UI
            mainScreenUserInterface = UserInterfaceXmlLoader.LoadUserInterface("Content/UserInterface/AutoMapMain.xml");
            mainScreenUserInterface.Fullscreen = true;
            mainScreenUserInterface.EnableScreencast();
            //Build the environment UI with a list of the typepaths
        }

    }
}
