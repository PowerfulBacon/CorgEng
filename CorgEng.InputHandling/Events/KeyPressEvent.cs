using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using GLFW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.Events
{
    public class KeyPressEvent : INetworkedEvent
    {

        public bool CanBeRaisedByClient => true;

        public Keys Key { get; set; }

        public ModifierKeys ModifierKeys { get; set; }

        public KeyPressEvent(Keys key, ModifierKeys modifierKeys)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }

        public void Deserialise(BinaryReader reader)
        {
            Key = (Keys)reader.ReadUInt16();
            ModifierKeys = (ModifierKeys)reader.ReadUInt16();
        }

        public void Serialise(BinaryWriter writer)
        {
            writer.Write((ushort)Key);
            writer.Write((ushort)ModifierKeys);
        }

        public int SerialisedLength()
        {
            return sizeof(ushort) * 2;
        }

    }
}
