using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.InputHandler
{
    public interface IInputHandler
    {

        void SetupInputHandler(Window targetWindow);

        void WindowUpdate(Window targetWindow);

        /// <summary>
        /// Add a keybind to a specific action.
        /// </summary>
        /// <param name="action">The action to bind the keybind to</param>
        /// <param name="key">The key to listen for.</param>
        /// <param name="priority">
        /// The priority of the keybind. If a keybind's action fails, then it will
        /// call the action of lower priority keybinds. If it passes, they will not be called.
        /// </param>
        /// <param name="modifierKeys">Any modifier keys that must be active along with the pressed key</param>
        void AddKeybind(string action, Keys key);

        /// <summary>
        /// Add a mouse button bind to a specific action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        /// <param name="priority"></param>
        /// <param name="modifierKeys"></param>
        void AddMouseButtonbind(string action, MouseButton key);

        /// <summary>
        /// Add a keybind which triggers when the mouse moves
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        void AddMouseMoveBind(string action);

        /// <summary>
        /// Add a keybind which triggers when the mouse wheel is scrolled.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        void AddMouseScrollBind(string action);

        /// <summary>
        /// Register the existance of an action. This allows it to be called by the keybind system
        /// </summary>
        /// <param name="action_key"></param>
        /// <param name="actionDelegate"></param>
        /// <param name="priority"></param>
        void AddButtonPressAction(string actionKey, Func<bool> actionDelegate, int priority);

        /// <summary>
        /// Register the existance of an action that triggers when a button is released.
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="actionDelegate">
        /// Takes in a single parameter, the time the button was held for 
        /// and returns a boolean value representing if the action was successful.
        /// </param>
        /// <param name="priority"></param>
        void AddButtonReleaseAction(string actionKey, Func<double, bool> actionDelegate, int priority);

        /// <summary>
        /// Register the existance of an action that triggers every frame a button is held.
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="actionDelegate">
        /// A delegate that takes in the button delta time as a parameter and returns
        /// a boolean value representing if the action was successful.
        /// </param>
        /// <param name="priority"></param>
        void AddButtonHoldAction(string actionKey, Func<double, bool> actionDelegate, int priority);

        /// <summary>
        /// Add an action to trigger when the mouse moves.
        /// While there is only 1 way you can bind to this action, the introduction
        /// of joysticks or something might make it so you can bind this in multiple
        /// ways. Not sure, but its here anyway.
        /// Takes in 3 parameters:
        ///  - Deltatime
        ///  - Mouse X
        ///  - Mouse Y
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="actionDelegate"></param>
        /// <param name="priority"></param>
        void AddMouseMoveAction(string actionKey, Func<double, double, double, bool> actionDelegate, int priority);

        /// <summary>
        /// Add in an action based on the scroll wheel.
        /// Takes in 2 parameters:
        ///  - Delta time
        ///  - Scroll delta
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="actionDelegate"></param>
        /// <param name="priority"></param>
        void AddMouseScrollAction(string actionKey, Func<double, double, bool> actionDelegate, int priority);

    }
}
