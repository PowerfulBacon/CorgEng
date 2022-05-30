using CorgEng.EntityComponentSystem.Events;
using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.Events
{
    public class KeyHeldEvent : Event
    {

        public Keys Key { get; }

        public override bool IsSynced => false;

        public KeyHeldEvent(Keys key)
        {
            Key = key;
        }
    }
}
