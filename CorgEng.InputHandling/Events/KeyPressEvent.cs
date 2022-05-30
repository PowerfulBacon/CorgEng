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

        public Keys Key { get; set; }

        public ModifierKeys ModifierKeys { get; set; }

        public override bool IsSynced => true;

        public KeyPressEvent(Keys key, ModifierKeys modifierKeys)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }

        public unsafe override byte[] Serialize()
        {
            //Get the key as a ushort pointer
            Keys tempKeys = Key;
            byte* keyPointer = (byte*)&tempKeys;
            return new byte[] {
                *keyPointer,
                *(keyPointer + 1),
                (byte)ModifierKeys
            };
        }

        public unsafe override void Deserialize(byte[] data)
        {
            fixed (byte* dataPointer = data)
            {
                Key = (Keys)(*(ushort*)dataPointer);
                ModifierKeys = (ModifierKeys)(*(dataPointer + 2));
            }
        }
    }
}
