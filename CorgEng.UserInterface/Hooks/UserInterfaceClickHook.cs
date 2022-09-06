using CorgEng.Core;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.UserInterface.Hooks;
using CorgEng.UserInterface.Components;
using CorgEng.UserInterface.Events;
using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Hooks
{
    [Dependency]
    internal class UserInterfaceClickHook : IUserInterfaceClickHook
    {

        internal static HashSet<UserInterfaceComponent> ScreencastingComopnents = new HashSet<UserInterfaceComponent>();

        /// <summary>
        /// Check if we clicked a user interface component.
        /// If so, stop passing on the clicks to click handlers.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="button"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        public bool TestUserInterfaceHook(double x, double y, MouseButton button, ModifierKeys modifiers)
        {
            foreach (UserInterfaceComponent component in ScreencastingComopnents)
            {
                if (component.Screencast((int)(CorgEngMain.GameWindow.Width * x), (int)(CorgEngMain.GameWindow.Height * (1 - y))) is UserInterfaceBox clickedBox)
                {
                    //Send a clicked signal to the component
                    new UserInterfaceClickEvent().Raise(clickedBox.ComponentHolder);
                    //Return true
                    return true;
                }
            }
            return false;
        }

    }
}
