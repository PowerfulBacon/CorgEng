using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class NewEntityEvent : Event
    {

        public override bool IsSynced => true;

        public int Identifier { get; set; }

        public NewEntityEvent(int identifier)
        {
            Identifier = identifier;
        }

        public override void Deserialize(byte[] data)
        {
            Identifier = BitConverter.ToInt32(data, 0);
        }

        public override byte[] Serialize()
        {
            return BitConverter.GetBytes(Identifier);
        }
    }
}
