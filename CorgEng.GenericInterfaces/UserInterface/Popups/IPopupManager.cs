using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Popups
{
    [NonDependency("IPopupManager is no longer used as a dependency and should be accessed from IWorld instead.")]
    public interface IPopupManager
    {

        /// <summary>
        /// Displays the provided UI component as a popup window.
        /// Returns the popup window which controls elements such as the position
        /// and rendering of the popup.
        /// </summary>
        /// <param name="userInterfaceWindow"></param>
        /// <returns></returns>
        IPopupWindow DisplayPopup(IUserInterfaceComponent userInterfaceWindow, int x, int y, int width, int height);

        /// <summary>
        /// Render the popups to the specified framebuffer.
        /// </summary>
        /// <param name="framebuffer"></param>
        void RenderToFramebuffer(uint framebuffer);

    }
}
