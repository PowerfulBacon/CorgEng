using CorgEng.EntityComponentSystem.Events;
using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.Events
{
    public class KeyReleaseEvent : Event
    {

        public Keys Key { get; set; }

        public ModifierKeys ModifierKeys { get; set; }

        public override bool NetworkedEvent => true;

        public KeyReleaseEvent(Keys key, ModifierKeys modifierKeys)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }

        public unsafe override byte[] Serialize()
        {
            short keyValue = (short)Key;
            byte* bytePointer = (byte*)&keyValue;
            return new byte[] {
                *bytePointer,
                *(bytePointer + 1),
                (byte)ModifierKeys
            };
        }

        public unsafe override void Deserialize(byte[] data)
        {
            fixed (byte* keyPointer = data)
            {
                short value = *keyPointer;
                Key = (Keys)value;
                ModifierKeys = *(ModifierKeys*)(keyPointer + 2);
            }
        }
    }
}
