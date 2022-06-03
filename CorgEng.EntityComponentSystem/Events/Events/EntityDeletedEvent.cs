using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class DeleteEntityEvent : INetworkedEvent
    {

        public byte[] Serialize()
        {
            return new byte[0];
        }

        public void Deserialize(byte[] data)
        { }
    }
}
