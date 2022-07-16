using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class NewEntityEvent : INetworkedEvent
    {

        public bool CanBeRaisedByClient => false;

        public uint Identifier { get; set; }

        public NewEntityEvent(uint identifier)
        {
            Identifier = identifier;
        }

        public void Deserialise(BinaryReader reader)
        {
            Identifier = reader.ReadUInt32();
        }

        public void Serialise(BinaryWriter writer)
        {
            writer.Write(Identifier);
        }

        public int SerialisedLength()
        {
            return sizeof(uint);
        }
    }
}
