using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.Events
{
    public class MouseReleaseEvent : IEvent
    {
        /// <summary>
        /// Cursor X position, from -1 to 1 (-1 being left side of screen and 1 being the right)
        /// </summary>
        public double CursorX { get; set; }

        /// <summary>
        /// Cursor Y position, from -1 to 1 (-1 being top side of screen and 1 being the bottom)
        /// </summary>
        public double CursorY { get; set; }

        public MouseButton MouseButton { get; set; }

        public ModifierKeys ModifierKeys { get; set; }

        /// <summary>
        /// The time that the mouse was held down for.
        /// </summary>
        public double HeldTime { get; set; }

        public MouseReleaseEvent(double cursorX, double cursorY, MouseButton mouseButton, ModifierKeys modifierKeys)
        {
            CursorX = cursorX;
            CursorY = cursorY;
            MouseButton = mouseButton;
            ModifierKeys = modifierKeys;
        }
    }
}
