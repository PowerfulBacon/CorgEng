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
        public void SetupInputHandler(Window targetWindow)
        {
            Glfw.SetKeyCallback(targetWindow, HandleKeyboardPress);
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
