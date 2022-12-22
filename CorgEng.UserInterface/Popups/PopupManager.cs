using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Popups
{
    [Dependency]
    internal class PopupManager : IPopupManager
    {

        [UsingDependency]
        private static IUserInterfaceComponentFactory UserInterfaceComponentFactory = null!;

        [UsingDependency]
        private static IAnchorFactory AnchorFactory = null!;

        [UsingDependency]
        private static IAnchorDetailFactory AnchorDetailFactory = null!;

        /// <summary>
        /// The root interface which all the popups are rendered on
        /// </summary>
        private static IUserInterfaceComponent popupRootInterface;

        [ModuleLoad(mainThread = true)]
        public static void InitialisePopupInterface()
        {
            popupRootInterface = UserInterfaceComponentFactory.CreateGenericUserInterfaceComponent(
                null,
                AnchorFactory.CreateAnchor(
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.RIGHT, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, 0),
                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PIXELS, 0)
                    ),
                null
                );
            popupRootInterface.Fullscreen = true;
            popupRootInterface.EnableScreencast();
        }

        public IPopupWindow DisplayPopup(IUserInterfaceComponent userInterfaceWindow, int x, int y, int width, int height)
        {
            //Create the new popup window and attach it to our root interface
            IPopupWindow createdWindow = new PopupWindow(userInterfaceWindow);
            createdWindow.X = x;
            createdWindow.Y = y;
            createdWindow.Width = width;
            createdWindow.Height = height;
            popupRootInterface.AddChild(userInterfaceWindow);
            return createdWindow;
        }

        public void RenderToFramebuffer(uint framebuffer)
        {
            popupRootInterface.DrawToFramebuffer(framebuffer);
        }
    }
}
