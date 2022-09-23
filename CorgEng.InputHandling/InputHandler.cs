using CorgEng.GenericInterfaces.InputHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLFW;
using CorgEng.InputHandling.Events;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.Core.Dependencies;
using CorgEng.EntityComponentSystem.Events;
using CorgEng.Core;
using CorgEng.InputHandling.ClickHandler;
using CorgEng.GenericInterfaces.UserInterface.Hooks;

namespace CorgEng.InputHandling
{
    [Dependency]
    public class InputHandler : IInputHandler
    {

        [UsingDependency]
        private static ILogger Logger;

        [UsingDependency]
        private static IUserInterfaceClickHook UserInterfaceClickHook;

        private Window window;

        private KeyCallback keyCallbackDelegate;
        private MouseButtonCallback mouseButtonCallback;
        private MouseCallback handleScrollCallback;
        private MouseCallback handleCursorMove;

        private HashSet<Keys> heldKeys = new HashSet<Keys>();

        public void SetupInputHandler(Window targetWindow)
        {
            Logger?.WriteLine($"Input handler setup to listen to {targetWindow}", LogType.LOG);
            keyCallbackDelegate = HandleKeyboardPress;
            mouseButtonCallback = HandleMousePress;
            handleScrollCallback = HandleScroll;
            handleCursorMove = HandleCursorMove;
            window = targetWindow;
            Glfw.SetKeyCallback(targetWindow, keyCallbackDelegate);
            Glfw.SetMouseButtonCallback(targetWindow, mouseButtonCallback);
            Glfw.SetScrollCallback(targetWindow, handleScrollCallback);
            Glfw.SetCursorPositionCallback(targetWindow, handleCursorMove);
        }

        private void HandleScroll(IntPtr window, double x, double y)
        {
            //Synchronous to prevent subsystem overloading, will not render the
            //next frame until this is handled.
            new MouseScrollEvent(y).RaiseGlobally(synchronous: true);
        }

        private void HandleCursorMove(IntPtr window, double x, double y)
        {
            //Synchronous to prevent subsystem overloading, will not render the
            //next frame until this is handled.
            new MouseMoveEvent(x / CorgEngMain.GameWindow.Width, y / CorgEngMain.GameWindow.Height).RaiseGlobally(synchronous: true);
        }

        private double mouseDownAt = 0;

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
                    mouseDownAt = CorgEngMain.Time;
                    return;
                case InputState.Release:
                    MouseReleaseEvent mouseReleaseEvent = new MouseReleaseEvent(x / width, y / height, button, modifiers);
                    mouseReleaseEvent.HeldTime = CorgEngMain.Time - mouseDownAt;
                    //Raise click events against the user interface (TODO: Add in event priorities)
                    if (UserInterfaceClickHook?.TestUserInterfaceHook(x / width, y / height, button, modifiers) ?? false)
                    {
                        return;
                    }
                    //Raise synchronously, so we can determine if the event was handled
                    mouseReleaseEvent.RaiseGlobally(true);
                    //Handle world clicks
                    if (!mouseReleaseEvent.Handled && mouseReleaseEvent.MouseButton == MouseButton.Left)
                    {
                        WorldClickHandler.HandleWorldClick(mouseReleaseEvent, CorgEngMain.GameWindow.Width, CorgEngMain.GameWindow.Height);
                    }
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
                    lock (heldKeys)
                    {
                        heldKeys.Add(key);
                    }
                    return;
                case InputState.Release:
                    KeyReleaseEvent keyReleaseEvent = new KeyReleaseEvent(key, mods);
                    keyReleaseEvent.RaiseGlobally();
                    lock (heldKeys)
                    {
                        heldKeys.Remove(key);
                    }
                    return;
            }
        }

        public void WindowUpdate(Window targetWindow)
        {
            lock (heldKeys)
            {
                foreach (Keys key in heldKeys)
                {
                    new KeyHeldEvent(key).RaiseGlobally();
                }
            }
        }

    }
}
