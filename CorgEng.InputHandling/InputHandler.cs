﻿using CorgEng.GenericInterfaces.InputHandler;
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

        private Dictionary<Keys, double> heldKeysDownAt = new Dictionary<Keys, double>();

        //================
        // Bounds Keybinds
        //================

        private Dictionary<Keys, string> boundKeyActions = new Dictionary<Keys, string>();
        private Dictionary<MouseButton, string> boundMouseActions = new Dictionary<MouseButton, string>();
        private List<string> boundMouseMoveActions = new List<string>();
        private List<string> boundScrollActions = new List<string>();

        //================
        // Bounds Actions
        //================
        //Dictionary of all button down actions
        private Dictionary<string, SortedList<int, Func<bool>>> buttonDownActions = new Dictionary<string, SortedList<int, Func<bool>>>();
        //Dictionary of all button up actions
        private Dictionary<string, SortedList<int, Func<double, bool>>> buttonUpActions = new Dictionary<string, SortedList<int, Func<double, bool>>>();
        //Dictionary of all button hold actions
        private Dictionary<string, SortedList<int, Func<double, bool>>> buttonHoldActions = new Dictionary<string, SortedList<int, Func<double, bool>>>();
        //Dictionary of all mouse move actions
        private Dictionary<string, SortedList<int, Func<double, double, double, bool>>> mouseMoveActions = new Dictionary<string, SortedList<int, Func<double, double, double, bool>>>();
        //Dictionary of all scroll actions
        private Dictionary<string, SortedList<int, Func<double, double, bool>>> scrollActions = new Dictionary<string, SortedList<int, Func<double, double, bool>>>();

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
            new MouseScrollEvent(y).RaiseGlobally(CorgEngMain.PrimaryWorld, synchronous: true);
            //Trigger the new actions
            foreach (string actionName in boundScrollActions)
            {
                SortedList<int, Func<double, double, bool>> actionFunctions;
                if (!scrollActions.TryGetValue(actionName, out actionFunctions))
                    return;
                foreach (Func<double, double, bool> actionFunction in actionFunctions.Values)
                {
                    //Call the function
                    if (actionFunction(x, y))
                        break;
                }
            }
        }

        private void HandleCursorMove(IntPtr window, double x, double y)
        {
            //Synchronous to prevent subsystem overloading, will not render the
            //next frame until this is handled.
            new MouseMoveEvent(x / CorgEngMain.GameWindow.Width, y / CorgEngMain.GameWindow.Height).RaiseGlobally(CorgEngMain.PrimaryWorld, synchronous: true);
            //Trigger the new actions
            foreach (string actionName in boundMouseMoveActions)
            {
                SortedList<int, Func<double, double, double, bool>> actionFunctions;
                if (!mouseMoveActions.TryGetValue(actionName, out actionFunctions))
                    return;
                foreach (Func<double, double, double, bool> actionFunction in actionFunctions.Values)
                {
                    //Call the function
                    if (actionFunction(CorgEngMain.DeltaTime, x / CorgEngMain.GameWindow.Width, y / CorgEngMain.GameWindow.Height))
                        break;
                }
            }
        }

        private double mouseDownAt = 0;

        private void HandleMousePress(IntPtr window, MouseButton button, InputState state, ModifierKeys modifiers)
        {
            try
            {
                string action;
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
                        //Raise click events against the user interface (TODO: Add in event priorities)
                        if (UserInterfaceClickHook?.TestUserInterfaceHook(x / width, y / height, button, modifiers) ?? false)
                        {
                            return;
                        }
                        mousePressEvent.RaiseGlobally(CorgEngMain.PrimaryWorld);
                        mouseDownAt = CorgEngMain.Time;
                        //Trigger the action
                        if (!boundMouseActions.TryGetValue(button, out action))
                            return;
                        SortedList<int, Func<bool>> actionFunctions;
                        if (!buttonDownActions.TryGetValue(action, out actionFunctions))
                            return;
                        foreach (Func<bool> actionFunction in actionFunctions.Values)
                        {
                            //Call the function
                            if (actionFunction())
                                break;
                        }
                        return;
                    case InputState.Release:
                        MouseReleaseEvent mouseReleaseEvent = new MouseReleaseEvent(x / width, y / height, button, modifiers);
                        mouseReleaseEvent.HeldTime = CorgEngMain.Time - mouseDownAt;
                        //Raise synchronously, so we can determine if the event was handled
                        mouseReleaseEvent.RaiseGlobally(CorgEngMain.PrimaryWorld, true);
                        //Handle world clicks
                        if (!mouseReleaseEvent.Handled && mouseReleaseEvent.MouseButton == MouseButton.Left)
                        {
                            SelectableSystem.HandleWorldClick(mouseReleaseEvent, CorgEngMain.GameWindow.Width, CorgEngMain.GameWindow.Height);
                        }
                        //Trigger the action
                        if (!boundMouseActions.TryGetValue(button, out action))
                            return;
                        SortedList<int, Func<double, bool>> releaseActions;
                        if (!buttonUpActions.TryGetValue(action, out releaseActions))
                            return;
                        foreach (Func<double, bool> actionFunction in releaseActions.Values)
                        {
                            //Call the function
                            if (actionFunction(mouseReleaseEvent.HeldTime))
                                break;
                        }
                        return;
                }
            }
            catch (System.Exception e)
            {
                Logger?.WriteLine(e, LogType.ERROR);
            }
        }

        private void HandleKeyboardPress(IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            try
            {
                //Ignore unknown keys
                if (key == Keys.Unknown)
                    return;
                string action;
                switch (state)
                {
                    case InputState.Press:
                        KeyPressEvent keyPressEvent = new KeyPressEvent(key, mods);
                        keyPressEvent.RaiseGlobally(CorgEngMain.PrimaryWorld);
                        lock (heldKeysDownAt)
                        {
                            heldKeysDownAt.Add(key, CorgEngMain.Time);
                        }
                        //Trigger the action
                        if (!boundKeyActions.TryGetValue(key, out action))
                            return;
                        SortedList<int, Func<bool>> actionFunctions;
                        if (!buttonDownActions.TryGetValue(action, out actionFunctions))
                            return;
                        foreach (Func<bool> actionFunction in actionFunctions.Values)
                        {
                            //Call the function
                            if (actionFunction())
                                break;
                        }
                        return;
                    case InputState.Release:
                        try
                        {
                            KeyReleaseEvent keyReleaseEvent = new KeyReleaseEvent(key, mods);
                            keyReleaseEvent.RaiseGlobally(CorgEngMain.PrimaryWorld);
                            //Trigger the action
                            if (!boundKeyActions.TryGetValue(key, out action))
                                return;
                            SortedList<int, Func<double, bool>> releaseActions;
                            if (!buttonUpActions.TryGetValue(action, out releaseActions))
                                return;
                            double timeHeld = CorgEngMain.Time - heldKeysDownAt[key];
                            foreach (Func<double, bool> actionFunction in releaseActions.Values)
                            {
                                //Call the function
                                if (actionFunction(timeHeld))
                                    break;
                            }
                        }
                        finally
                        {
                            lock (heldKeysDownAt)
                            {
                                heldKeysDownAt.Remove(key);
                            }
                        }
                        return;
                }
            }
            catch (System.Exception e)
            {
                Logger?.WriteLine(e, LogType.ERROR);
            }
        }

        public void WindowUpdate(Window targetWindow)
        {
            lock (heldKeysDownAt)
            {
                foreach (Keys key in heldKeysDownAt.Keys)
                {
                    //new KeyHeldEvent(key).RaiseGlobally();
                    string action;
                    if (!boundKeyActions.TryGetValue(key, out action))
                        continue;
                    SortedList<int, Func<double, bool>> actionFunctions;
                    if (!buttonHoldActions.TryGetValue(action, out actionFunctions))
                        continue;
                    foreach (Func<double, bool> actionFunction in actionFunctions.Values)
                    {
                        //Call the function
                        if (actionFunction(CorgEngMain.DeltaTime))
                            break;
                    }
                }
            }
        }

        public void AddKeybind(string action, Keys key)
        {
            lock (boundKeyActions)
            {
                boundKeyActions.Add(key, action);
            }
        }

        public void AddMouseButtonbind(string action, MouseButton key)
        {
            lock (boundMouseActions)
            {
                boundMouseActions.Add(key, action);
            }
        }

        public void AddMouseMoveBind(string action)
        {
            lock (boundMouseMoveActions)
            {
                boundMouseMoveActions.Add(action);
            }
        }

        public void AddMouseScrollBind(string action)
        {
            lock (boundScrollActions)
            {
                boundScrollActions.Add(action);
            }
        }

        public void AddButtonPressAction(string actionKey, Func<bool> actionDelegate, int priority)
        {
            lock (buttonDownActions)
            {
                if (!buttonDownActions.ContainsKey(actionKey))
                    buttonDownActions.Add(actionKey, new SortedList<int, Func<bool>>());
                buttonDownActions[actionKey].Add(priority, actionDelegate);
            }
        }

        public void AddButtonReleaseAction(string actionKey, Func<double, bool> actionDelegate, int priority)
        {
            lock (buttonUpActions)
            {
                if (!buttonUpActions.ContainsKey(actionKey))
                    buttonUpActions.Add(actionKey, new SortedList<int, Func<double, bool>>());
                buttonUpActions[actionKey].Add(priority, actionDelegate);
            }
        }

        public void AddButtonHoldAction(string actionKey, Func<double, bool> actionDelegate, int priority)
        {
            lock (buttonHoldActions)
            {
                if (!buttonHoldActions.ContainsKey(actionKey))
                    buttonHoldActions.Add(actionKey, new SortedList<int, Func<double, bool>>());
                buttonHoldActions[actionKey].Add(priority, actionDelegate);
            }
        }

        public void AddMouseMoveAction(string actionKey, Func<double, double, double, bool> actionDelegate, int priority)
        {
            lock (mouseMoveActions)
            {
                if (!mouseMoveActions.ContainsKey(actionKey))
                    mouseMoveActions.Add(actionKey, new SortedList<int, Func<double, double, double, bool>>());
                mouseMoveActions[actionKey].Add(priority, actionDelegate);
            }
        }

        public void AddMouseScrollAction(string actionKey, Func<double, double, bool> actionDelegate, int priority)
        {
            lock (scrollActions)
            {
                if (!scrollActions.ContainsKey(actionKey))
                    scrollActions.Add(actionKey, new SortedList<int, Func<double, double, bool>>());
                scrollActions[actionKey].Add(priority, actionDelegate);
            }
        }

        public void GetCursorPosition(out double x, out double y)
        {
            Glfw.GetCursorPosition(window, out x, out y);
        }
    }
}
