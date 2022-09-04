using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Hooks
{
    public interface IUserInterfaceClickHook
    {

        /// <summary>
        /// Test if user interface hooks have been applied.
        /// </summary>
        /// <param name="x">X value between 0 and 1, depending on screen width</param>
        /// <param name="y">Y value between 0 and 1, depending on screen width</param>
        /// <param name="button"></param>
        /// <param name="modifiers"></param>
        /// <returns>Returns true if the hook should block input handling.</returns>
        bool TestUserInterfaceHook(double x, double y, MouseButton button, ModifierKeys modifiers);

    }
}
