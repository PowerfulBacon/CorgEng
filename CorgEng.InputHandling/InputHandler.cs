using CorgEng.GenericInterfaces.InputHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLFW;
using CorgEng.InputHandling.Events;

namespace CorgEng.InputHandling
{
    public class InputHandler : IInputHandler
    {

        private Window window;

        public void SetupInputHandler(Window targetWindow)
        {
            window = targetWindow;
            Glfw.SetKeyCallback(targetWindow, HandleKeyboardPress);
            Glfw.SetMouseButtonCallback(targetWindow, HandleMousePress);
        }

        private void HandleMousePress(IntPtr window, MouseButton button, InputState state, ModifierKeys modifiers)
        {
            double x;
            double y;
            int width;
            int height;
            Glfw.GetCursorPosition(this.window, out x, out y);
            Glfw.GetWindowSize(this.window, out width, out height);
            switch (state)
            {
                case InputState.Press:
                    MousePressEvent mousePressEvent = new MousePressEvent(x / width, y / height, button, modifiers);
                    mousePressEvent.RaiseGlobally();
                    return;
                case InputState.Release:
                    MouseReleaseEvent mouseReleaseEvent = new MouseReleaseEvent(x / width, y / height, button, modifiers);
                    mouseReleaseEvent.RaiseGlobally();
                    return;
            }
        }

        private void HandleKeyboardPress(IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            //Ignore unknown keys
            if (key == Keys.Unknown)
                return;
            switch (state)
            {
                case InputState.Press:
                    KeyPressEvent keyPressEvent = new KeyPressEvent(key, mods);
                    keyPressEvent.RaiseGlobally();
                    return;
                case InputState.Release:
                    KeyReleaseEvent keyReleaseEvent = new KeyReleaseEvent(key, mods);
                    keyReleaseEvent.RaiseGlobally();
                    return;
            }
        }

    }
}
