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
    public class KeyHeldEvent : IEvent
    {

        public Keys Key { get; }

        public KeyHeldEvent(Keys key)
        {
            Key = key;
        }
    }
}
