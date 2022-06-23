using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class NewEntityEvent : INetworkedEvent
    {

        public uint Identifier { get; set; }

        public NewEntityEvent(uint identifier)
        {
            Identifier = identifier;
        }

        public void Deserialize(byte[] data)
        {
            Identifier = BitConverter.ToUInt32(data, 0);
        }

        public byte[] Serialize()
        {
            return BitConverter.GetBytes(Identifier);
        }
    }
}
