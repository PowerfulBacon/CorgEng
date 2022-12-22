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
    [Obsolete("Standard input events have been replaced with an improved input handling system with rebindable events. See IInputHandler. Due to performance, this event is no longer fired.", true)]
    public class KeyHeldEvent : IEvent
    {

        public Keys Key { get; }

        public KeyHeldEvent(Keys key)
        {
            Key = key;
        }
    }
}
