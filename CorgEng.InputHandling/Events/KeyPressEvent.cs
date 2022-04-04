using CorgEng.EntityComponentSystem.Events;
using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.Events
{
    public class KeyPressEvent : Event
    {

        Keys Key { get; set; }

        ModifierKeys ModifierKeys { get; set; }

        public KeyPressEvent(Keys key, ModifierKeys modifierKeys)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }
    }
}
